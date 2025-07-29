using Unity.Burst;
using Unity.NetCode;
using Unity.Physics;
using Unity.Entities;

namespace Unity.Vehicles.Samples
{
    [BurstCompile]
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(PredictedFixedStepSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ServerSimulation)]
    public partial struct VehicleControlPredictionSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            VehicleControlSystem.VehicleControlJob job = new VehicleControlSystem.VehicleControlJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
                PhysicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld,
                WheelControlLookup = SystemAPI.GetComponentLookup<WheelControl>(true),
                EngineControlLookup = SystemAPI.GetComponentLookup<EngineStartStop>(false),
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);

            state.Dependency = new VehicleControlSystem.VehicleEngineControlJob
            {
                EngineControlLookup = SystemAPI.GetComponentLookup<EngineStartStop>(false),
            }.Schedule(state.Dependency);
        }
    }
}