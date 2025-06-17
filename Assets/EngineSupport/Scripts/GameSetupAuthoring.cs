using Unity.Entities;
using UnityEditor;
using UnityEngine;

public class GameSetupAuthoring : MonoBehaviour
{
    
    public GameObject Controller;
    public GameObject Item;
    
    
    class Baker : Baker<GameSetupAuthoring>
    {
        public override void Bake(GameSetupAuthoring authoring)
        {
            GameConfigSpawner gameConfig = new GameConfigSpawner
            {
                Controller = GetEntity(authoring.Controller, TransformUsageFlags.Dynamic),
                Item = GetEntity(authoring.Item, TransformUsageFlags.Dynamic)
            };
            
            var GameConfigEntity = GetEntity(TransformUsageFlags.None);
            AddComponent(GameConfigEntity, gameConfig);
        }
    }
}

public struct GameConfigSpawner : IComponentData
{
    public Entity Controller;
    public Entity Item;
}


public partial struct GameSpawnerConfig : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var controller = SystemAPI.GetSingleton<GameConfigSpawner>().Controller;
        var item = SystemAPI.GetSingleton<GameConfigSpawner>().Item;        
        
        // Associate the instantiated prefab with the connected client's assigned NetworkId
        //commandBuffer.SetComponent(player, new GhostOwner { NetworkId = networkId.Value});
        // obtener el network id del player ponerselo al GhostOwner del controller? ?

        state.EntityManager.Instantiate(controller);
        state.EntityManager.Instantiate(item);
        
        state.Enabled = false; // Disable this system after spawning the controller

    }
}
