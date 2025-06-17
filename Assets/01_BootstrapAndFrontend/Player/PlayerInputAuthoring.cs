using Unity.Entities;
using UnityEngine;

namespace Samples.HelloNetcode
{

    [DisallowMultipleComponent]
    public class PlayerInputAuthoring : MonoBehaviour
    {
        [SerializeField]
        private float MoveSpeed;
        [SerializeField]
        private float JumpImpulse;
        class Baker : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<PlayerInput>(entity);
                AddComponent(entity, 
                    new PlayerParameters 
                    { 
                        MoveSpeed = authoring.MoveSpeed, 
                        JumpImpulse = authoring.MoveSpeed, 
                    });
            }
        }
    }
}