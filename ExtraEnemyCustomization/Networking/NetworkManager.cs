using EECustom.Networking.Events;
using System.Collections.Generic;
using System.Linq;
using System;
using EECustom.Networking.Replicators;
using EECustom.Events;
using Enemies;
using Agents;
using EECustom.Managers;
using EECustom.Networking.Handlers;

namespace EECustom.Networking
{
    public static class NetworkManager
    {
        public static EnemyAgentModeReplicator EnemyAgentModeState { get; private set; } = new();
        public static EnemyAnimEvent EnemyAnim { get; private set; } = new();

        internal static void Initialize()
        {
            EnemyEvents.Spawned += EnemySpawned;

            EnemyAgentModeState.Initialize();
            EnemyAnim.Setup();
        }

        private static void EnemySpawned(EnemyAgent agent)
        {
            EnemyAgentModeState.Register(agent.GlobalID, default, (newState) =>
            {
                ConfigManager.Current.FireAgentModeChangedEvent(agent, newState.mode);
            });

            var updater = agent.gameObject.AddComponent<EnemyStateUpdater>();
            updater.Agent = agent;
        }
    }
}