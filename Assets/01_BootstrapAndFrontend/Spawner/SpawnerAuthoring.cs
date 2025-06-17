using Unity.Entities;
using UnityEngine;

namespace Samples.HelloNetcode
{
    public struct Spawner : IComponentData
    {
        public Entity Player;
    }

    [DisallowMultipleComponent]
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject Player;

        class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Spawner 
                {
                    Player = GetEntity(authoring.Player, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}