using EEC.Events;
using Enemies;

namespace EEC.Managers.Properties
{
    [CallConstructorOnLoad]
    public static class EnemyDeathManager
    {
        private static readonly Dictionary<ushort, bool> _killedLookup = new(30);

        static EnemyDeathManager()
        {
            EnemyEvents.Killed += Killed;
            EnemyEvents.Despawn += Despawn;
            LevelEvents.LevelCleanup += OnLevelCleanup;
        }

        private static void Killed(EnemyAgent agent)
        {
            _killedLookup[agent.GlobalID] = true;
        }

        private static void Despawn(EnemyAgent agent)
        {
            _killedLookup.Remove(agent.GlobalID);
        }

        private static void OnLevelCleanup()
        {
            _killedLookup.Clear();
        }

        public static bool WasKilled(EnemyAgent enemy) => WasKilled(enemy.GlobalID);
        public static bool WasKilled(ushort id)
        {
            return _killedLookup.GetValueOrDefault(id);
        }
    }
}
