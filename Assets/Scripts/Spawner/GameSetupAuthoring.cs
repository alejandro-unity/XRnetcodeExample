using Unity.Entities;
using UnityEngine;

namespace Unity.Samples.EngineSupport
{
    public struct GameConfigSpawner : IComponentData
    {
        public Entity Controller;
        public Entity Item;
        public Entity Player;
    }

    public class GameSetupAuthoring : MonoBehaviour
    {
        public GameObject Player;
        public GameObject Controller;
        public GameObject Item;


        class Baker : Baker<GameSetupAuthoring>
        {
            public override void Bake(GameSetupAuthoring authoring)
            {
                GameConfigSpawner gameConfig = new GameConfigSpawner
                {
                    Controller = GetEntity(authoring.Controller, TransformUsageFlags.Dynamic),
                    Item = GetEntity(authoring.Item, TransformUsageFlags.Dynamic),
                    Player = GetEntity(authoring.Player, TransformUsageFlags.Dynamic),
                };

                var GameConfigEntity = GetEntity(TransformUsageFlags.None);
                AddComponent(GameConfigEntity, gameConfig);
            }
        }
    }
}