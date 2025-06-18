using Unity.Entities;
using UnityEngine;

namespace Unity.Samples.EngineSupport
{
    class TagItemBaker : MonoBehaviour
    {
        class TagHandBakerBaker : Baker<TagItemBaker>
        {
            public override void Bake(TagItemBaker authoring)
            {

                // add the tag component to the entity
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var tag = default(ItemTag);
                AddComponent(entity, tag);
            }
        }

    }


    public struct ItemTag : IComponentData
    {
    }
}