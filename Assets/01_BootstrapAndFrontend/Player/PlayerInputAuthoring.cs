using UnityEngine;
using Unity.Entities;

namespace Samples.HelloNetcode
{
    [DisallowMultipleComponent]
    public class PlayerInputAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float MoveSpeed;
        class Baker : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerInput>(entity);
                AddComponent(entity, new PlayerParameters { MoveSpeed = authoring.MoveSpeed });
            }
        }
    }
}