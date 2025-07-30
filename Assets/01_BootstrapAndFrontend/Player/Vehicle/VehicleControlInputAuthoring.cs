using Unity.Entities;
using UnityEngine;

class VehicleControlInputAuthoring : MonoBehaviour
{
    class VehicleControlInputAuthoringBaker : Baker<VehicleControlInputAuthoring>
    {
        public override void Bake(VehicleControlInputAuthoring authoring)
        {
            var entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent<VehicleControlInput>(entity);
        }
    }
}


