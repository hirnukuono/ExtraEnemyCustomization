using EEC.Events;
using EEC.Managers;
using EEC.Networking.Events;
using EEC.Networking.Replicators;
using Enemies;

namespace EEC.Networking
{
    public static class NetworkManager
    {
        public const ulong LOWEST_STEAMID64 = 0x0110000100000001;

        public static EnemyAgentModeReplicator EnemyAgentModeState { get; private set; } = new();
        public static EnemyAnimEvent EnemyAnim { get; private set; } = new();

        internal static void Initialize()
        {
            EnemyEvents.Spawned += EnemySpawned;
            EnemyEvents.Despawn += EnemyDespawn;

            EnemyAgentModeState.Initialize();
            EnemyAnim.Setup();
        }

        private static void EnemySpawned(EnemyAgent agent)
        {
            if (agent.TryGetSpawnData(out var spawnData))
            {
                var defaultState = new EnemyAgentModeReplicator.State()
                {
                    mode = spawnData.mode
                };

                EnemyAgentModeState.Register(agent.GlobalID, defaultState, (newState) =>
                {
                    ConfigManager.FireAgentModeChangedEvent(agent, newState.mode);
                });
            }
        }

        private static void EnemyDespawn(EnemyAgent agent)
        {
            EnemyAgentModeState.Deregister(agent.GlobalID);
        }
    }
}