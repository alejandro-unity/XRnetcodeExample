using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace EngineSupport.Scripts
{
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [BurstCompile]
    public partial struct UpdateArmDataSystem : ISystem
    {

        public void OnCreate(ref SystemState state)
        {
            state.Enabled = false; 
        }
        
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (linkedEntityGroup, playerTransform,armData) 
                     in SystemAPI.Query<DynamicBuffer<LinkedEntityGroup>, RefRW<LocalTransform>, RefRO<ArmData>>().WithAll<PlayerTag>())
            {
                foreach (var childArm in linkedEntityGroup)
                {
                    if (SystemAPI.HasComponent<ControllerTag>(childArm.Value))
                    {
                        var childTransform = state.EntityManager.GetComponentData<LocalTransform>(childArm.Value);
                        childTransform.Position = armData.ValueRO.Position;
                        state.EntityManager.SetComponentData<LocalTransform>(childArm.Value, childTransform);
                    }
                }
            }
        }
    }
}