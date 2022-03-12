using EECustom.Events;
using EECustom.Managers;
using EECustom.Networking.Events;
using EECustom.Networking.Replicators;
using Enemies;

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
                ConfigManager.FireAgentModeChangedEvent(agent, newState.mode);
            });
        }
    }
}