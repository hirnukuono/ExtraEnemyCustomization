using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                health = Mathf.Max(0.0f, agent.Damage.Health)
            });
        }

        public struct State
        {
            public float maxHealth;
            public float health;
        }
    }
}
