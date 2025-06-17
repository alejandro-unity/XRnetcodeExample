using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

//[GhostComponent(PrefabType = GhostPrefabType.AllPredicted, OwnerSendType = SendToOwnerType.SendToOwner)]
[GhostComponent]
public struct ArmData : IComponentData
{
    [GhostField]
    public float3 Position;
    public quaternion Rotation;
    public float3 LinearVelocity;
    public float3 AngularVelocity;
    public ushort LastClientInputTick; // To track which client input was processed
}

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))] 
// TO Check UpdateInGroup(typeof(GhostPredictionSystemGroup)) 
public partial struct PlayerArmInputSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (localTransform, armData, entity) in 
                 SystemAPI.Query<RefRO<LocalTransform>, RefRW<ArmData>>().WithAll<GhostOwnerIsLocal>().WithEntityAccess())
        {
            ref var armDataRef = ref armData.ValueRW; 
            var newPosition = localTransform.ValueRO.Position + new float3(1, 0, 0); 
            armDataRef.Position = newPosition; 
        }
    }
}