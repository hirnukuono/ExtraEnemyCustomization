using Enemies;
using SNetwork;

namespace EECustom.Networking.Events
{
    public sealed class EnemyAnimEvent : SyncedEvent<EnemyAnimEvent.Packet>
    {
        public override string GUID => "EAE";

        public override void Receive(Packet packet)
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

        public struct Packet
        {
            public ushort enemyID;
            public int animHash;
            public bool pauseAI;
            public float crossfadeTime;
        }
    }

    
}