using Unity.Entities;

namespace Samples.HelloNetcode
{
    public struct Spawner : IComponentData
    {
        public Entity Player;
    }
}