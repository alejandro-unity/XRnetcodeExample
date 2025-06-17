using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

public struct JointConnected : IComponentData {}

[BurstCompile]
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ConnectJointSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state) 
    {
        state.RequireForUpdate<Player>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var commandBuffer = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        foreach (var (player, entity) in SystemAPI.Query<RefRO<Player>>()
            .WithEntityAccess()
            .WithNone<JointConnected>()) 
        {
            var item = player.ValueRO.Item;
            var controller = player.ValueRO.Controller;
            var linkedEntityBuffer = state.EntityManager.GetBuffer<LinkedEntityGroup>(item);
            foreach (var child in linkedEntityBuffer)
            {
                if (SystemAPI.HasComponent<PhysicsConstrainedBodyPair>(child.Value))
                {
                    commandBuffer.SetComponent(child.Value, new PhysicsConstrainedBodyPair(item, controller, true));
                }
            }
            
            commandBuffer.AddComponent<JointConnected>(entity);
        }

        commandBuffer.Playback(state.EntityManager);

    }
}
