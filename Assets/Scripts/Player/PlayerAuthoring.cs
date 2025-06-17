using Unity.Entities;
using UnityEngine;

public struct Player : IComponentData
{
    public Entity Controller;
    public Entity Item;
}

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
