using Unity.NetCode;
using Unity.Entities;
using UnityEngine.SceneManagement;

namespace Unity.Vehicles.Samples
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [UpdateBefore(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(VehicleControlPredictionSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.Default | WorldSystemFilterFlags.ClientSimulation)]
    public partial struct NetcodeMinimalVehicleControlSystem : ISystem
    {
        public void OnCreate(ref SystemState state) 
        {
            state.Enabled = SceneManager.GetActiveScene().name.ToLower().Contains("multiplayer");
        }

        public void OnUpdate(ref SystemState state)
        {
            MinimalInputActions.DefaultMapActions defaultMapActions = MinimalInputResources.InputActions.DefaultMap;

            foreach (var (controller, vehicleControl) in
                     SystemAPI.Query<MinimalPlayerController, RefRW<VehicleControl>>())
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
}