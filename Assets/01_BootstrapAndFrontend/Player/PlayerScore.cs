using Samples.HelloNetcode;
using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;


[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
partial struct PlayerScore : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerParameters>();
    }

    public void OnUpdate(ref SystemState state)
    {
        foreach (var (player, owner) in SystemAPI.Query<RefRW<PlayerParameters>, RefRO<GhostOwner>>()) 
        {
            if (!player.ValueRO.ScoreUI.IsValid())
            {
                player.ValueRW.ScoreUI.Value = GameObject.FindFirstObjectByType<ScoreUI>();
                continue;
            }

            player.ValueRO.ScoreUI.Value.UpdateScore(owner.ValueRO.NetworkId % 2, player.ValueRO.Score);
        }
    }
}
