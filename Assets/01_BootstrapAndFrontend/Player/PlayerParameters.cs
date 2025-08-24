using Unity.Entities;
using Unity.NetCode;

namespace Samples.HelloNetcode
{
    public struct PlayerParameters : IComponentData 
    {
        [GhostField] public int Score;
        public float MoveSpeed;
    }
}