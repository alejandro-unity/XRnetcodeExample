using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;

namespace Samples.HelloNetcode
{
    public struct Spawner : IComponentData
    {
        public Entity Ball;
        public Entity Player;
        public float3 BallPosition;
        public float2 MapSize;
    }

    public struct SpawnPoint : IBufferElementData
    {
        public float3 Position;
        public quaternion Rotation;
    }

    /// <summary>
    /// Flag component, denoting whether or not a Player Character Controller (CC) has been spawned
    /// for a given connection.
    /// </summary>
    public struct PlayerSpawned : IComponentData { }

    /// <summary>
    ///     Convenience: This allows us to trivially fetch the connection entity associated with
    ///     this player character controller entity.
    /// </summary>
    public struct ConnectionOwner : IComponentData
    {
        public Entity Entity;
    }

    public struct Ball : IComponentData
    {
    }

    public struct BallPending : IComponentData { }
}