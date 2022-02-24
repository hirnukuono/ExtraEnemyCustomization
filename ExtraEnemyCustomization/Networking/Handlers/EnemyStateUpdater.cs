using Agents;
using EECustom.Attributes;
using EECustom.Networking.Replicators;
using Enemies;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Networking.Handlers
{
    [InjectToIl2Cpp]
    internal class EnemyStateUpdater : MonoBehaviour
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

                NetworkManager.EnemyAgentModeState.SetState(Agent.GlobalID, new EnemyState()
                {
                    mode = newMode
                });
            }
        }

        internal void OnDestroy()
        {
            NetworkManager.EnemyAgentModeState.Deregister(Agent.GlobalID);
            Agent = null;
        }
    }
}
