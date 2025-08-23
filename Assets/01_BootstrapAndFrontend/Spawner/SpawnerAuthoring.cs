using Unity.Entities;
using UnityEngine;

namespace Samples.HelloNetcode
{

    [DisallowMultipleComponent]
    public class SpawnerAuthoring : MonoBehaviour
    {
        [SerializeField]
        private GameObject Player;
        [SerializeField]
        private GameObject Ball;
        [SerializeField]
        private Transform BallPosition;
        [SerializeField]
        private Transform [] SpawnPoints;

        class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Spawner 
                {
                    BallPosition = authoring.BallPosition.position,
                    Ball = GetEntity(authoring.Ball, TransformUsageFlags.Dynamic),
                    Player = GetEntity(authoring.Player, TransformUsageFlags.Dynamic),
                });

                var spawnPoints = AddBuffer<SpawnPoint>(entity);
                foreach (var transform in authoring.SpawnPoints) 
                {
                    spawnPoints.Add(new SpawnPoint { Position = transform.position, Rotation = transform.rotation });
                }
            }
        }
    }
}