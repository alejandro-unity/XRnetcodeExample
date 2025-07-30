using Unity.Entities;
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
                var requestedPlayType = RequestedPlayType;
                World world = default;
                if (requestedPlayType != PlayType.Client)
                {
                    world = CreateServerWorld("ServerWorld");
                }

                if (requestedPlayType != PlayType.Server)
                {
                    world = CreateClientWorld("ClientWorld");
                }

                World.DefaultGameObjectInjectionWorld = world;
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
