using Unity.Entities;
using Unity.Mathematics;
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
        private Vector2 MapSize = new Vector2(150,100);
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
                    MapSize = authoring.MapSize,
                });

                var spawnPoints = AddBuffer<SpawnPoint>(entity);
                foreach (var transform in authoring.SpawnPoints) 
                {
                    spawnPoints.Add(new SpawnPoint { Position = transform.position, Rotation = transform.rotation });
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(transform.position, new Vector3(MapSize.x, 1, MapSize.y));
        }
    }
}