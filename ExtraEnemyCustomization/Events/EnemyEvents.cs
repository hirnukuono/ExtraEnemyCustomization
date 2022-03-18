using Enemies;

namespace EEC.Events
{
    public delegate void EnemyAgentHandlerWithSpawnData(EnemyAgent agent, pEnemySpawnData spawnData);
    public delegate void EnemyAgentHandler(EnemyAgent agent);

    public static class EnemyEvents
    {
        public static event EnemyAgentHandlerWithSpawnData Spawn;

        public static event EnemyAgentHandler Spawned;

        public static event EnemyAgentHandler Despawn;

        public static event EnemyAgentHandler Despawned;

        internal static void OnSpawn(EnemyAgent agent, pEnemySpawnData spawnData)
        {
            Spawn?.Invoke(agent, spawnData);
        }

        internal static void OnSpawned(EnemyAgent agent)
        {
            Spawned?.Invoke(agent);
        }

        internal static void OnDespawn(EnemyAgent agent)
        {
            Despawn?.Invoke(agent);
        }

        internal static void OnDespawned(EnemyAgent agent)
        {
            Despawned?.Invoke(agent);
        }
    }
}