using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct PlayerInput : IInputComponentData
{
    public int Horizontal;
    public int Vertical;

    public FixedString512Bytes ToFixedString() => $"→{Horizontal} ↑{Vertical}";
}

[DisallowMultipleComponent]
public class PlayerInputAuthoring : MonoBehaviour
{
    class PlayerInputBaking : Baker<PlayerInputAuthoring>
    {
        public override void Bake(PlayerInputAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerInput>(entity);
        }
    }
}

[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial struct PlayerInputSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkStreamInGame>();
        state.RequireForUpdate<GameConfigSpawner>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var playerInput in SystemAPI.Query<RefRW<PlayerInput>>().WithAll<GhostOwnerIsLocal>())
        {
            playerInput.ValueRW = default;
            if (Input.GetKey("left") || Input.GetKey(KeyCode.LeftArrow))
                playerInput.ValueRW.Horizontal -= 1;
            if (Input.GetKey("right") || Input.GetKey(KeyCode.RightArrow))
                playerInput.ValueRW.Horizontal += 1;
            if (Input.GetKey("down") || Input.GetKey(KeyCode.DownArrow))
                playerInput.ValueRW.Vertical -= 1;
            if (Input.GetKey("up") || Input.GetKey(KeyCode.UpArrow))
                playerInput.ValueRW.Vertical += 1;
        }
        
    }
}
