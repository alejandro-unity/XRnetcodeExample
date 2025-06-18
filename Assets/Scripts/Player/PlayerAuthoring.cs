using Unity.Entities;
using UnityEngine;

namespace Unity.Samples.EngineSupport
{
    [DisallowMultipleComponent]
    public class PlayerAuthoring : MonoBehaviour
    {
        class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Player>(entity);
            }
        }
    }
    public struct Player : IComponentData
    {
        public Entity Controller;
        public Entity Item;
    }

}
