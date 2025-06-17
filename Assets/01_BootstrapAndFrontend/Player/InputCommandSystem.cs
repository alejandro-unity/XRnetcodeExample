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
            bool left = Input.GetKey(KeyCode.A);
            bool right = Input.GetKey(KeyCode.D);
            bool down = Input.GetKey(KeyCode.S);
            bool up = Input.GetKey(KeyCode.W);
            bool jump = Input.GetKeyDown(KeyCode.Space);

            foreach (var inputData in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>()) 
            {
                inputData.ValueRW = default;

                if (jump)
                    inputData.ValueRW.Jump.Set();
                if (left)
                    inputData.ValueRW.Horizontal -= 1;
                if (right)
                    inputData.ValueRW.Horizontal += 1;
                if (down)
                    inputData.ValueRW.Vertical -= 1;
                if (up)
                    inputData.ValueRW.Vertical += 1;
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

            foreach (var (input, velocity) in SystemAPI.Query<RefRO<PlayerInput>, RefRW<PhysicsVelocity>>().WithAll<Simulate>())
            {
                float3 forward = new float3(0, 0, 1);
                velocity.ValueRW.Linear += forward * input.ValueRO.Horizontal;
            }


        }
    }
}