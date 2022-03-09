using AIGraph;
using EECustom.Utils;
using Enemies;
using System;

namespace EECustom
{
    public static class EnemyAgentExtension
    {
        public static void AddOnDeadOnce(this EnemyAgent agent, Action onDead)
        {
            var called = false;
            agent.add_OnDeadCallback(new Action(() =>
            {
                if (called)
                    return;

                onDead?.Invoke();
                called = true;
            }));
        }

        public static T RegisterOrGetProperty<T>(this EnemyAgent agent) where T : class, new()
        {
            return EnemyProperty<T>.RegisterOrGet(agent);
        }

        public static bool TryGetProperty<T>(this EnemyAgent agent, out T property) where T : class, new()
        {
            return EnemyProperty<T>.TryGet(agent, out property);
        }

        public static pEnemySpawnData GetSpawnData(this EnemyAgent agent)
        {
            return agent.m_replicator.Cast<EnemyReplicator>().GetSpawnData();
        }

        public static AIG_CourseNode GetSpawnedNode(this EnemyAgent agent)
        {
            var spawnData = agent.m_replicator.Cast<EnemyReplicator>().GetSpawnData();
            if (!spawnData.courseNode.TryGet(out var node))
                return null;

            return node;
        }
    }
}