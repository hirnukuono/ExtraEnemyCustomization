using Enemies;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;
using UnhollowerBaseLib;

namespace EECustom.Networking.Events
{
    public class EnemyAnimEvent : SyncedEvent<EnemyAnimPacket>
    {
        public override void Receive(EnemyAnimPacket packet)
        {
            SNet_Replication.TryGetReplicator(packet.enemyID, out var replicator);
            if (replicator == null)
                return;

            if (replicator.ReplicatorSupplier == null)
                return;

            var enemySync = replicator.ReplicatorSupplier.TryCast<EnemySync>();
            if (enemySync == null)
                return;

            var enemyAgent = enemySync.m_agent;
            if (enemyAgent == null)
                return;

            var animator = enemyAgent.Locomotion.m_animator;
            var navAgent = enemyAgent.AI.m_navMeshAgent;

            animator.CrossFadeInFixedTime(packet.animHash, packet.crossfadeTime);
            if (packet.pauseAI)
            {
                if (navAgent.isOnNavMesh)
                    navAgent.isStopped = true;
            }
        }
    }

    public struct EnemyAnimPacket
    {
        public ushort enemyID;
        public int animHash;
        public bool pauseAI;
        public float crossfadeTime;
    }
}
