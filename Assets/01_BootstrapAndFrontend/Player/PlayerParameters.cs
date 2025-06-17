using Unity.Entities;
using Unity.NetCode;

namespace Samples.HelloNetcode
{
    public struct PlayerParameters : IInputComponentData 
    {
        public float MoveSpeed;
        public float JumpImpulse;
    }
}