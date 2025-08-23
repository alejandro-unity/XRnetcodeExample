using Unity.NetCode;

namespace Samples.HelloNetcode
{
    public struct PlayerInput : IInputComponentData
    {
        public int Vertical;
    }
}