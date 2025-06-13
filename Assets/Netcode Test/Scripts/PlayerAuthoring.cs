using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public struct Player : IComponentData
{
    public Entity Character;
    public Entity Controller;

    public Entity Hand;
    
}

[DisallowMultipleComponent]
public class PlayerAuthoring : MonoBehaviour
{
    [SerializeField] private GameObject _character;
    [SerializeField] private GameObject _controller;

    [SerializeField] private GameObject _hand;
    class Baker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Player>(entity, new()
            {
                Character = GetEntity(authoring._character, TransformUsageFlags.Dynamic),
                Controller = GetEntity(authoring._controller, TransformUsageFlags.Dynamic),
                Hand = GetEntity(authoring._hand, TransformUsageFlags.Dynamic),

            });
        }
    }
}