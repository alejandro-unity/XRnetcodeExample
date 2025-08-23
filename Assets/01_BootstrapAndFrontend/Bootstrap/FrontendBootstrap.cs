using Unity.NetCode;

namespace Samples.HelloNetcode
{
    [UnityEngine.Scripting.Preserve]
    public class FrontendBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            AutoConnectPort = 7979;
            // Create the appropriate worlds, which we can then load sub-scenes directly into:
            CreateDefaultClientServerWorlds();

            return true;
        }
    }
}
