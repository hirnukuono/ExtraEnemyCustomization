using Agents;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Networking.Replicators
{
    public sealed class EnemyAgentModeReplicator : StateReplicator<EnemyAgentModeReplicator.State>
    {
        public override bool ClearOnLevelCleanup => true;

        public override string GUID => "EMD";

        public void SetState(ushort id, AgentMode newMode)
        {
            SetState(id, new State()
            {
                mode = newMode
            });
        }

        public struct State
        {
            public AgentMode mode;
        }
    }
}
