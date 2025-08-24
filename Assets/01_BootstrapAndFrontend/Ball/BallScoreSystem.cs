using Samples.HelloNetcode;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(TransformSystemGroup), OrderLast = true)]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct BallScoreSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Ball>();
        state.RequireForUpdate<Spawner>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        var ballScoreJob = new BallScoreJob
        {
            BallEntity = SystemAPI.GetSingletonEntity<Ball>(),
            MapWidth = SystemAPI.GetSingleton<Spawner>().MapSize.x,
            ECB = ecb.AsParallelWriter(),
            LocalToWorldComponentLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true),
            BallComponentLookup = SystemAPI.GetComponentLookup<Ball>(true),
        };

        state.Dependency = ballScoreJob.ScheduleParallel(state.Dependency);
    }

    [WithAll(typeof(PlayerParameters))]
    [BurstCompile]
    partial struct BallScoreJob : IJobEntity 
    {
        public Entity BallEntity;
        public float MapWidth;
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly]
        public ComponentLookup<LocalToWorld> LocalToWorldComponentLookup;
        [ReadOnly]
        public ComponentLookup<Ball> BallComponentLookup;

        [BurstCompile]
        private void Execute([EntityIndexInQuery] int index,  Entity entity, ref PlayerParameters playerParameters)
        {
            var ballPosition = LocalToWorldComponentLookup[BallEntity];
            var ball = BallComponentLookup[BallEntity];

            var playerPosition = LocalToWorldComponentLookup[entity];
            float3 forward = playerPosition.Forward;
            float3 toBall = math.normalize(ballPosition.Position - playerPosition.Position);
            float dot = math.dot(forward, toBall);
            bool isBallBehind = dot < 0f;
            bool isBallBehindAPlayer = math.abs(ballPosition.Position.x) > MapWidth * 0.75f;

            if (isBallBehindAPlayer) 
            {
                if (!isBallBehind && playerParameters.Score < 10)
                {
                    playerParameters.Score++;
                    if (playerParameters.Score < 10) 
                    {
                        ECB.DestroyEntity(index, BallEntity);
                    }
                    
                }
            }
        }
    }
}
