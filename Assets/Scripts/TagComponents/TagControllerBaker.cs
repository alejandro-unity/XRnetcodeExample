using Unity.Entities;
using UnityEngine;

namespace Unity.Samples.EngineSupport
{
    class ESController : MonoBehaviour
    {
        class ESControllerBaker : Baker<ESController>
        {
            public override void Bake(ESController authoring)
            {

                // add the tag component to the entity
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                var tag = default(ControllerTag);
                AddComponent(entity, tag);
            }
        }
    }

    public struct ControllerTag : IComponentData
    {
    }
}
