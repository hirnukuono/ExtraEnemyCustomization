using Agents;
using EECustom.Attributes;
using Enemies;
using SNetwork;
using UnityEngine;

namespace EECustom.Networking.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class EnemyStateUpdater : MonoBehaviour
    {
        public EnemyAgent Agent;

        private AgentMode _currentAgentMode = AgentMode.Off;

        internal void Update()
        {
            if (!SNet.IsMaster)
                return;

            var newMode = Agent.AI.Mode;

            if (_currentAgentMode != newMode)
            {
                _currentAgentMode = newMode;

                NetworkManager.EnemyAgentModeState.SetState(Agent.GlobalID, newMode);
            }
        }

        internal void OnDestroy()
        {
            NetworkManager.EnemyAgentModeState.Deregister(Agent.GlobalID);
            Agent = null;
        }
    }
}