using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using UnityEngine;

namespace Samples.HelloNetcode
{
    // Sample keypress inputs every frame and add them to the input component for
    // processing later.
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [AlwaysSynchronizeSystem]
    public partial class GatherAutoCommandsSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<PlayerInput>();
            RequireForUpdate<NetworkStreamInGame>();
        }

        protected override void OnUpdate()
        {
            bool down = Input.GetKey(KeyCode.S);
            bool up = Input.GetKey(KeyCode.W);
            
            foreach (var inputData in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>()) 
            {
                inputData.ValueRW = default;

                if (up)
                    inputData.ValueRW.Vertical += 1;
                if (down)
                    inputData.ValueRW.Vertical -= 1;
            }
        }
    }

    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [BurstCompile]
    public partial struct PlayerInputMovementSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInput>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (input, velocity, parameters) in SystemAPI.Query<RefRO<PlayerInput>, RefRW<PhysicsVelocity>, RefRO<PlayerParameters>>().WithAll<Simulate>())
            {
                float3 movement = (math.forward() * input.ValueRO.Vertical) * parameters.ValueRO.MoveSpeed;
                velocity.ValueRW.Linear.z = movement.z;
            }
        }
    }
}