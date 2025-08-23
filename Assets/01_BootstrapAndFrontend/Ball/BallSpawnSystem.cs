using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

namespace Samples.HelloNetcode
{
    /// <summary>
    /// Handles spawning of the Ball entity.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct BallSpawnSystem : ISystem
    {
        private EntityQuery m_PlayersQuery;
        private EntityQuery m_BallQuery;

        // Prefab de la bola que se instanciará
        Entity m_BallPrefab;

        // Referencia a una instancia de Random para generar valores aleatorios
        private NativeReference<Random> m_RandomReference;
        // Lookups para obtener datos de componentes, en este caso, el transform del prefab
        ComponentLookup<LocalTransform> m_LocalTransformLookup;

        public void OnCreate(ref SystemState state)
        {
            m_PlayersQuery = SystemAPI.QueryBuilder().WithAll<NetworkId>().WithAll<PlayerSpawned>().Build();
            m_BallQuery = SystemAPI.QueryBuilder().WithAll<Ball>().Build();
            state.RequireForUpdate(m_PlayersQuery);
            state.RequireForUpdate<Spawner>();

            // Inicializa la referencia aleatoria con una semilla única
            var fileTimeUtc = System.DateTime.UtcNow.ToFileTimeUtc();
            m_RandomReference = new NativeReference<Random>(Random.CreateFromIndex((uint)fileTimeUtc), Allocator.Persistent);

            // Inicializa el lookup de componentes
            m_LocalTransformLookup = state.GetComponentLookup<LocalTransform>(true);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            // Libera la memoria de la referencia aleatoria cuando el sistema se destruye
            m_RandomReference.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (m_PlayersQuery.CalculateEntityCount() < 2 || m_BallQuery.CalculateEntityCount() > 0)
            {
                return;
            }

            // Obtiene el prefab de la bola del singleton BallSpawner si aún no está asignado
            if (m_BallPrefab == Entity.Null)
            {
                var ballSpawner = SystemAPI.GetSingleton<Spawner>();
                m_BallPrefab = ballSpawner.Ball;

                if (m_BallPrefab == Entity.Null)
                    return;
            }

            // Crea un EntityCommandBuffer para ejecutar comandos de forma segura en un Job
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            // Actualiza el lookup antes de que se ejecute el job
            m_LocalTransformLookup.Update(ref state);

            // Obtiene el singleton de NetworkTime para saber el tick actual del servidor
            var tick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

            // Obtiene el singleton de ClientServerTickRate para el delta time fijo
            SystemAPI.TryGetSingleton<ClientServerTickRate>(out var tickRate);
            tickRate.ResolveDefaults();
            var fixedDeltaTime = 1.0f / (float)tickRate.SimulationTickRate;

            // Job para generar la bola
            var spawnBallJob = new SpawnBallJob
            {
                ecb = ecb,
                random = m_RandomReference,
                tick = tick,
                ballPrefab = m_BallPrefab,
                localTransformLookup = m_LocalTransformLookup,
                fixedDeltaTime = fixedDeltaTime,
            };

            // Programa el Job y establece la dependencia
            state.Dependency = spawnBallJob.Schedule(state.Dependency);
        }

        [BurstCompile]
        private partial struct SpawnBallJob : IJob
        {
            public EntityCommandBuffer ecb;
            public NativeReference<Random> random;
            public NetworkTick tick;
            public Entity ballPrefab;
            [ReadOnly] public ComponentLookup<LocalTransform> localTransformLookup;
            public float fixedDeltaTime;

            public void Execute()
            {
                var rand = random.Value;

                // Define la posición inicial fija. Ahora está en el plano XZ.
                var position = new float3(0, 6, 0);

                // Genera un ángulo de rotación aleatorio para la dirección en el plano XZ.
                // La rotación se aplica sobre el eje Y para girar en el plano XZ.
                var angle = rand.NextFloat(-0.0f, 359.0f);

                // Obtiene la escala original del prefab.
                var originalScale = localTransformLookup[ballPrefab].Scale;

                // Crea el transform con la posición y rotación aleatorias.
                // La rotación se aplica sobre el eje Y.
                var trans = LocalTransform.FromPositionRotationScale(
                    position,
                    quaternion.RotateY(math.radians(angle)),
                    originalScale);

                // Define una velocidad base para la bola.
                var ballVelocity = 100.0f;

                // Calcula el vector de velocidad basado en la rotación.
                // Para movimiento en el plano XZ, el vector de dirección es (0, 0, 1) antes de la rotación.
                // Se multiplica la rotación por este vector para obtener la dirección final.
                // Calcula el vector de velocidad basado en la rotación en el plano XZ.
                // El vector de dirección "hacia adelante" es (0, 0, 1).
                var direction = math.mul(trans.Rotation, new float3(0, 0, 1));
                // Define la velocidad en el plano XZ usando las componentes X y Z de la dirección.
                var vel = new float3(direction.x, 0, direction.z) * ballVelocity;

                // Instancia la entidad de la bola.
                var newBall = ecb.Instantiate(ballPrefab);

                // Establece el transform y el componente Ball en la nueva entidad.
                ecb.SetComponent(newBall, trans);
                ecb.SetComponent(newBall, new Ball
                {
                    InitialPosition = position,
                    InitialVelocity = vel,
                    InitialAngle = angle,
                    SpawnTick = tick
                });

                // Añade el componente de velocidad física para el movimiento.
                // El 'Linear' es el vector de velocidad que calculaste.
                // El 'Angular' es 0, ya que no se necesita rotación en este caso.
                ecb.SetComponent(newBall, new PhysicsVelocity { Linear = vel, Angular = float3.zero });

                // Actualiza el estado de Random para la próxima ejecución.
                random.Value = rand;
            }
        }
    }
}