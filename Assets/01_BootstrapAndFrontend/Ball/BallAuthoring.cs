using Samples.HelloNetcode;
using Unity.Entities;
using UnityEngine;

class BallAuthoring : MonoBehaviour
{
    class BallAuthoringBaker : Baker<BallAuthoring>
    {
        public override void Bake(BallAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<Ball>(entity);
        }
    }
}


