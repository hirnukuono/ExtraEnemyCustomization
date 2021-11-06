using Enemies;

namespace EECustom.Events
{
    public delegate void ScoutDetectionHandler(EnemyAgent enemyAgent, ScoutAntennaDetection detection);
    public delegate void ScoutAntennaHandler(EnemyAgent enemyAgent, ScoutAntennaDetection detection, ScoutAntenna antenna);

    public static class ScoutAntennaSpawnEvent
    {
        public static event ScoutDetectionHandler DetectionSpawn;
        public static event ScoutAntennaHandler AntennaSpawn;

        internal static void OnDetectionSpawn(EnemyAgent enemyAgent, ScoutAntennaDetection detection)
        {
            DetectionSpawn?.Invoke(enemyAgent, detection);
        }

        internal static void OnAntennaSpawn(EnemyAgent enemyAgent, ScoutAntennaDetection detection, ScoutAntenna antenna)
        {
            AntennaSpawn?.Invoke(enemyAgent, detection, antenna);
        }
    }
}