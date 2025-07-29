using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Vehicles;

[GhostComponentVariation(typeof(VehicleControlData))]
public struct VehicleControlData_GhostVariant
{
    [GhostField()]
    public float SteeringPosition;
    [GhostField()]
    public int TransmissionCurrentGear;
    [GhostField()]
    public float TransmissionCurrentGearRatio;
    [GhostField()]
    public float EngineAngularVelocity;
    [GhostField()]
    public bool EngineIsRunning;
}

[GhostComponentVariation(typeof(WheelOnVehicle))]
public struct WheelOnVehicle_GhostVariant
{
    [GhostField(SendData = false)]
    public int WheelProtectorChildColliderIndex;
    [GhostField(SendData = false)]
    public RigidTransform SuspensionLocalTransform;
    [GhostField(SendData = false)]
    public Entity AxlePairedWheelEntity;
    [GhostField(SendData = false)]
    public int AxlePairedWheelIndex;
    [GhostField(SendData = false)]
    public Entity Entity;
    [GhostField(SendData = false)]
    public Wheel Wheel;
    [GhostField(SendData = false)]
    public float MotorTorque;
    [GhostField(SendData = false)]
    public float BrakeTorque;
    [GhostField(SendData = false)]
    public float AngularVelocityLimit;

    [GhostField()]
    public float SteerAngle;
    [GhostField()]
    public float AngularVelocity;
    [GhostField()]
    public float RotationAngle;
    [GhostField()]
    public float SuspensionLength;
    [GhostField(SendData = false)]
    public float VisualSuspensionLength;

    [GhostField(SendData = false)]
    public bool IsGrounded;
    [GhostField(SendData = false)]
    public ColliderCastHit WheelHit;
    [GhostField(SendData = false)]
    public float3 SuspensionImpulse;
    [GhostField(SendData = false)]
    public float3 FrictionImpulse;
    [GhostField()]
    public float2 FrictionSlip;
    [GhostField(SendData = false)]
    public float2 FrictionSpeed;

    [GhostField()]
    public bool StaticFrictionReferenceIsSet;
    [GhostField()]
    public float3 StaticFrictionRefPosition;
    [GhostField(SendData = false)]
    public bool DisableStaticFrictionSingleFrame;
}