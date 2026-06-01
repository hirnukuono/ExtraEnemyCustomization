using Enemies;
using UnityEngine;

namespace EEC.Networking.Replicators
{
    public sealed class EnemyHealthInfoReplicator : StateReplicator<EnemyHealthInfoReplicator.State>
    {
        public override bool ClearOnLevelCleanup => true;

        public override string GUID => "EHI";

        public void UpdateInfo(EnemyAgent agent)
        {
            SetState(agent.GlobalID, new State()
            {
                maxHealth = agent.Damage.HealthMax,
                health = Mathf.Max(0f, agent.Damage.Health)
            });
        }

        public struct State
        {
            public float maxHealth;
            public float health;
        }
    }
}
