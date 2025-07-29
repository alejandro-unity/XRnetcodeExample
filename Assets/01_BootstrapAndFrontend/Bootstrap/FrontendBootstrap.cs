using Unity.NetCode;
using UnityEngine.SceneManagement;

namespace Samples.HelloNetcode
{
    [UnityEngine.Scripting.Preserve]
    public class FrontendBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            if (SceneManager.GetActiveScene().name.ToLower().Contains("multiplayer"))
            {
                AutoConnectPort = 7979;
                CreateDefaultClientServerWorlds();
            }
            else 
            {
                AutoConnectPort = 0;
                CreateLocalWorld(defaultWorldName);
            }

            return true;
        }
    }
}
