using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Collections;
using Unity.Burst;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        var builder = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<Simulate>()
            .WithAll<PlayerInput>()
            .WithAllRW<LocalTransform>();

        var query = state.GetEntityQuery(builder);
        state.RequireForUpdate(query);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var moveJob = new MovePlayerJob
        {
            tick = SystemAPI.GetSingleton<NetworkTime>().ServerTick,
            Speed = SystemAPI.Time.DeltaTime * 4,
            TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(false)
        };

        state.Dependency = moveJob.Schedule(state.Dependency);
        
    }

    [BurstCompile]
    [WithAll(typeof(Simulate))]
    [WithAll(typeof(JointConnected))]

    partial struct MovePlayerJob : IJobEntity
    {
        public ComponentLookup<LocalTransform> TransformLookup;
        public NetworkTick tick;
        public float Speed;

        public void Execute(Entity entity, in PlayerInput playerInput, in Player player)
        {
            var moveInput = new float2(playerInput.Horizontal, playerInput.Vertical);
            moveInput = math.normalizesafe(moveInput) * Speed;
            var transform = TransformLookup[entity];
            transform.Position += new float3(moveInput.x, 0, moveInput.y);
            TransformLookup[entity] = transform;

            var controllerTransform = TransformLookup[player.Controller];
            controllerTransform.Position = transform.Position + math.forward();
            TransformLookup[player.Controller] = controllerTransform;
        }
    }
}
