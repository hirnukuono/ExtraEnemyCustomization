using Agents;
using EEC.Events;
using EEC.Managers;
using EEC.Networking.Events;
using EEC.Networking.Replicators;
using EEC.Utils.Unity;
using Enemies;
using SNetwork;
using System.Collections;
using UnityEngine;

namespace EEC.Networking
{
    public static class NetworkManager
    {
        public const ulong LOWEST_STEAMID64 = 0x0110000100000001;

        public static EnemyAgentModeReplicator EnemyAgentModeState { get; private set; } = new();
        public static EnemyHealthInfoReplicator EnemyHealthState { get; private set; } = new();
        public static EnemyAnimEvent EnemyAnim { get; private set; } = new();

        internal static void Initialize()
        {
            EnemyEvents.Spawned += EnemySpawned;
            EnemyEvents.Despawn += EnemyDespawn;

            EnemyAgentModeState.Initialize();
            EnemyHealthState.Initialize();
            EnemyAnim.Setup();
        }

        private static void EnemySpawned(EnemyAgent agent)
        {
            if (agent.TryGetSpawnData(out var spawnData))
            {
                var agentModeState = new EnemyAgentModeReplicator.State()
                {
                    mode = spawnData.mode
                };

                EnemyAgentModeState.Register(agent.GlobalID, agentModeState, (newState) =>
                {
                    ConfigManager.FireAgentModeChangedEvent(agent, newState.mode);
                });
            }


            var agentHealthState = new EnemyHealthInfoReplicator.State()
            {
                maxHealth = agent.Damage.HealthMax,
                health = agent.Damage.Health
            };

            EnemyHealthState.Register(agent.GlobalID, agentHealthState, (newState) =>
            {
                EnemyDamageEvents.OnHealthUpdated(agent, newState.maxHealth, newState.health);
            });

            agent.AI.StartCoroutine(CheckHealth(agent));
        }

        private static IEnumerator CheckHealth(EnemyAgent agent)
        {
            var fixedUpdateYielder = WaitFor.FixedUpdate;
            var health = agent.Damage.Health;
            while (true)
            {
                if (SNet.IsMaster)
                {
                    var newHealth = agent.Damage.Health;
                    if (!Mathf.Approximately(health, newHealth))
                    {
                        EnemyHealthState.UpdateInfo(agent);
                        health = newHealth;
                    }
                }
                yield return fixedUpdateYielder;
            }
        }

        private static void EnemyDespawn(EnemyAgent agent)
        {
            EnemyAgentModeState.Deregister(agent.GlobalID);
        }
    }
}