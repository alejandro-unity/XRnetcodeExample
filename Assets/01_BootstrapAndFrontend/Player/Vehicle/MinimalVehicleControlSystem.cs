using Unity.NetCode;
using Unity.Entities;
using UnityEngine.SceneManagement;

namespace Unity.Vehicles.Samples
{
    // Sample keypress inputs every frame and add them to the input component for
    // processing later.
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct SetInputCommandsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.Enabled = SceneManager.GetActiveScene().name.ToLower().Contains("multiplayer");
        }

        public void OnUpdate(ref SystemState state)
        {
            MinimalInputActions.DefaultMapActions defaultMapActions = MinimalInputResources.InputActions.DefaultMap;

            foreach (var vehicleControl in SystemAPI.Query<RefRW<VehicleControlInput>>())
            {
                vehicleControl.ValueRW.RawSteeringInput = defaultMapActions.Steering.ReadValue<float>();
                vehicleControl.ValueRW.RawThrottleInput = defaultMapActions.Throttle.ReadValue<float>();
                vehicleControl.ValueRW.RawBrakeInput = defaultMapActions.Brake.ReadValue<float>();
                vehicleControl.ValueRW.HandbrakeInput = defaultMapActions.Handbrake.ReadValue<float>();
                vehicleControl.ValueRW.ShiftUpInput = default;
                if (defaultMapActions.ShiftUp.WasPressedThisFrame())
                {
                    vehicleControl.ValueRW.ShiftUpInput = true;
                }

                vehicleControl.ValueRW.ShiftDownInput = default;
                if (defaultMapActions.ShiftDown.WasPressedThisFrame())
                {
                    vehicleControl.ValueRW.ShiftDownInput = true;
                }

                vehicleControl.ValueRW.EngineStartStopInput = default;
                if (defaultMapActions.EngineStartStop.WasPressedThisFrame())
                {
                    vehicleControl.ValueRW.EngineStartStopInput = true;
                }
            }
        }
    }
    
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(PredictedFixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(CopyCommandBufferToInputSystemGroup))]
    [UpdateBefore(typeof(VehicleControlPredictionSystem))]
    public partial struct NetcodeMinimalVehicleControlSystem : ISystem
    {
        public void OnCreate(ref SystemState state) 
        {
            state.Enabled = SceneManager.GetActiveScene().name.ToLower().Contains("multiplayer");
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (controller, vehicleControl) in SystemAPI.Query<RefRO<VehicleControlInput>, RefRW<VehicleControl>>())
            {
                var defaultMapActions = controller.ValueRO;
                vehicleControl.ValueRW.RawSteeringInput = defaultMapActions.RawSteeringInput;
                vehicleControl.ValueRW.RawThrottleInput = defaultMapActions.RawThrottleInput;
                vehicleControl.ValueRW.RawBrakeInput = defaultMapActions.RawBrakeInput;
                vehicleControl.ValueRW.HandbrakeInput = defaultMapActions.HandbrakeInput;
                vehicleControl.ValueRW.ShiftUpInput = default;

                if (defaultMapActions.ShiftUpInput)
                {
                    vehicleControl.ValueRW.ShiftUpInput = true;
                }

                vehicleControl.ValueRW.ShiftDownInput = default;
                
                if (defaultMapActions.ShiftDownInput)
                {
                    vehicleControl.ValueRW.ShiftDownInput = true;
                }

                vehicleControl.ValueRW.EngineStartStopInput = default;
                if (defaultMapActions.EngineStartStopInput)
                {
                    vehicleControl.ValueRW.EngineStartStopInput = true;
                }
            }
        }
    }
}