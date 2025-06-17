using Unity.Entities;
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