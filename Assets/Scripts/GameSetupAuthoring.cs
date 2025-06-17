using Unity.Entities;
using Unity.Physics;
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
        var prefabController = SystemAPI.GetSingleton<GameConfigSpawner>().Controller;
        var prefabItem = SystemAPI.GetSingleton<GameConfigSpawner>().Item;        
        
        // Associate the instantiated prefab with the connected client's assigned NetworkId
        //commandBuffer.SetComponent(player, new GhostOwner { NetworkId = networkId.Value});
        // obtener el network id del player ponerselo al GhostOwner del controller? ?

        var controller = state.EntityManager.Instantiate(prefabController);
        var item = state.EntityManager.Instantiate(prefabItem);
        //get linkedEntity buffer from item entity
        var linkedEntityBuffer = state.EntityManager.GetBuffer<LinkedEntityGroup>(item);
        PhysicsConstrainedBodyPair joint = default(PhysicsConstrainedBodyPair);
        foreach (var child in linkedEntityBuffer)
        {
            if (SystemAPI.HasComponent<PhysicsConstrainedBodyPair>(child.Value))
            {
                joint = state.EntityManager.GetComponentData<PhysicsConstrainedBodyPair>(child.Value);
                joint = new PhysicsConstrainedBodyPair(item, controller, true);
                state.EntityManager.SetComponentData(child.Value, joint);
                Debug.Log($"created joint between {item.Index} and {controller.Index}");
            }
        }
        
        

        
        
        
        // this is working a client or server 
        
        state.Enabled = false; // Disable this system after spawning the controller

    }
}
