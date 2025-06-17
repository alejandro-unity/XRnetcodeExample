using Unity.Collections;
using Unity.NetCode;

namespace Samples.HelloNetcode
{
    public struct PlayerInput : IInputComponentData
    {
        public int Horizontal;
        public int Vertical;
        public InputEvent Jump;

        public FixedString512Bytes ToFixedString() => $"h:{Horizontal},v:{Vertical},jump:{Jump.ToFixedString()}";
    }
}