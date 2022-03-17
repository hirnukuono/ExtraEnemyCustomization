using AIGraph;
using EEC.Utils;
using Enemies;
using SNetwork;
using System;

namespace EEC
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
            return agent.Sync.Replicator.Cast<EnemyReplicator>().GetSpawnData();
        }

        public static bool TryGetEnemyGroup(this EnemyAgent agent, out EnemyGroup group)
        {
            var spawnData = agent.GetSpawnData();
            if (spawnData.groupReplicatorKey == 0)
            {
                group = null;
                return false;
            }

            if (SNet_Replication.TryGetReplicator(spawnData.groupReplicatorKey, out var replicator))
            {
                group = null;
                return false;
            }

            if (replicator.ReplicatorSupplier == null)
            {
                group = null;
                return false;
            }

            group = replicator.ReplicatorSupplier.TryCast<EnemyGroup>();
            return group != null;
        }

        public static AIG_CourseNode GetSpawnedNode(this EnemyAgent agent)
        {
            var spawnData = agent.Sync.Replicator.Cast<EnemyReplicator>().GetSpawnData();
            if (!spawnData.courseNode.TryGet(out var node))
                return null;

            return node;
        }
    }
}