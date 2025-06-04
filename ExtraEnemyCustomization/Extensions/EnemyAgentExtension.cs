using AIGraph;
using EEC.Managers.Properties;
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

        public static bool TryGetSpawnData(this EnemyAgent agent, out pEnemySpawnData spawnData)
        {
            return EnemySpawnDataManager.TryGet(agent.GlobalID, out spawnData);
        }

        public static bool TryGetEnemyGroup(this EnemyAgent agent, out EnemyGroup group)
        {
            if (!agent.TryGetSpawnData(out var spawnData))
            {
                group = null;
                return false;
            }

            if (spawnData.groupReplicatorKey == 0)
            {
                group = null;
                return false;
            }

            if (!SNet_Replication.TryGetReplicator(spawnData.groupReplicatorKey, out var replicator))
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

        public static bool CanUseAbilities(this EnemyAgent agent)
        {
            return agent.Locomotion.CurrentStateEnum switch
            {
                ES_StateEnum.BirtherGiveBirth
                or ES_StateEnum.Jump
                or ES_StateEnum.Hitreact
                or ES_StateEnum.HitReactFlyer
                or ES_StateEnum.HibernateWakeUp
                or ES_StateEnum.Hibernate
                or ES_StateEnum.ScoutDetection
                or ES_StateEnum.ScoutScream
                or ES_StateEnum.Scream
                or ES_StateEnum.ScreamFlyer
                or ES_StateEnum.StrikerMelee
                or ES_StateEnum.StuckInGlue
                or ES_StateEnum.TriggerFogSphere
                or ES_StateEnum.Dead
                or ES_StateEnum.DeadFlyer
                or ES_StateEnum.DeadSquidBoss => false,
                _ => true
            };
        }

        public static bool IsStopped(this EnemyAgent agent)
        {
            return agent.Locomotion.CurrentStateEnum switch
            {
                ES_StateEnum.BirtherGiveBirth
                or ES_StateEnum.ShooterAttack
                or ES_StateEnum.StrikerAttack
                or ES_StateEnum.ShooterAttackFlyer
                or ES_StateEnum.TankAttack
                or ES_StateEnum.StrikerMelee
                or ES_StateEnum.ScoutScream
                or ES_StateEnum.Scream
                or ES_StateEnum.ScreamFlyer
                or ES_StateEnum.Hitreact
                or ES_StateEnum.HitReactFlyer
                or ES_StateEnum.StuckInGlue
                or ES_StateEnum.ScoutDetection
                or ES_StateEnum.Dead
                or ES_StateEnum.DeadFlyer
                or ES_StateEnum.DeadSquidBoss => true,
                _ => false
            };
        }

        public static bool CanApplyRootMotion(this EnemyAgent agent)
        {
            return agent.Locomotion.CurrentStateEnum switch
            {
                ES_StateEnum.ClimbLadder
                or ES_StateEnum.StuckInGlue
                or ES_StateEnum.PathMove
                or ES_StateEnum.PathMoveFlyer => false,
                _ => true
            };
        }
    }
}