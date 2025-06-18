using Unity.Entities;
using UnityEngine;

namespace Unity.Samples.EngineSupport
{
    class ESPlayer : MonoBehaviour
    {
        class ESPlayerBaker : Baker<ESPlayer>
        {
            public override void Bake(ESPlayer authoring)
            {
                // add the tag component to the entity
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerTag>(entity);
            }
        }
    }

    public struct PlayerTag : IComponentData
    {
    }
}