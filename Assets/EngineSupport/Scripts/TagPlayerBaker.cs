using Unity.Entities;
using UnityEngine;

class ESPlayer : MonoBehaviour
{
}

class ESPlayerBaker : Baker<ESPlayer>
{
    public override void Bake(ESPlayer authoring)
    {
        // add the tag component to the entity
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        var tag = default(PlayerTag);
        AddComponent(entity, tag);
    }
}

public struct PlayerTag : IComponentData
{
}
