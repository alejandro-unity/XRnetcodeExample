using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Physics;

namespace Unity.Samples.EngineSupport
{
    public struct JointConnected : IComponentData { }

    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation | WorldSystemFilterFlags.ClientSimulation)]
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
                if (player.ValueRO.Item == Entity.Null || player.ValueRO.Controller == Entity.Null)
                    continue;

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

    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial struct ConnectJointClientSystem : ISystem
    {
        public EntityQuery m_ItemLocal;
        public EntityQuery m_ControllerLocal;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Player>();
            m_ItemLocal =
                state.GetEntityQuery(ComponentType.ReadOnly<ItemTag>(), ComponentType.ReadOnly<GhostOwnerIsLocal>());
            m_ControllerLocal =
                state.GetEntityQuery(ComponentType.ReadOnly<ControllerTag>(), ComponentType.ReadOnly<GhostOwnerIsLocal>());

            state.RequireForUpdate(m_ItemLocal);
            state.RequireForUpdate(m_ControllerLocal);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var itemEntity = m_ItemLocal.ToEntityArray(Allocator.Temp)[0];
            var contoller = m_ControllerLocal.ToEntityArray(Allocator.Temp)[0];

            foreach (var player in SystemAPI.Query<RefRW<Player>>())
            {
                player.ValueRW.Item = itemEntity;
                player.ValueRW.Controller = contoller;
            }
        }
    }
}