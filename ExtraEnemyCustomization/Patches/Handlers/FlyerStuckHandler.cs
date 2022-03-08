using Agents;
using EECustom.Attributes;
using EECustom.Utils;
using Enemies;
using UnityEngine;

namespace EECustom.Patches.Handlers
{
    [InjectToIl2Cpp]
    public sealed class FlyerStuckHandler : MonoBehaviour
    {
        public EnemyAgent Agent;
        public float UpdateInterval = 2.0f;
        public int RetryCount = 4;

        private Vector3 _firstPosition;
        private Vector2 _lastGoalXZ;
        private Timer _timer;
        private int _tryCount = -1;
        private bool _shouldCheck = true;

        internal void FixedUpdate()
        {
            if (_shouldCheck)
            {
                if (Agent.AI.Mode != AgentMode.Agressive)
                    return;

                if (!_timer.TickAndCheckDone())
                    return;

                _timer.Reset(UpdateInterval);

                if (_tryCount == -1)
                {
                    _firstPosition = Agent.Position;
                    _tryCount = 0;
                    return;
                }

                if (Vector3.Distance(_firstPosition, Agent.Position) < 0.1f)
                {
                    _tryCount++;

                    if (_tryCount >= RetryCount)
                    {
                        Logger.Debug("Flyer was stuck in Place!");
                        Agent.m_replicator.Despawn();
                    }
                }
                else
                {
                    _shouldCheck = false;
                }
            }
            else
            {
                var goal = Agent.AI.NavmeshAgentGoal;
                var goalXZ = new Vector2(goal.x, goal.z);
                var goalDeltaSqr = (goalXZ - _lastGoalXZ).sqrMagnitude;
                if (goalDeltaSqr < 0.1f)
                {
                    var state = (EB_States)Agent.AI.m_behaviour.CurrentState.ENUM_ID;
                    if (state == EB_States.InCombat) //Possibly Stuck
                    {
                        _tryCount = -1;
                        _shouldCheck = true;
                    }
                }
                else
                {
                    _tryCount = -1;
                    _shouldCheck = false;
                }

                _lastGoalXZ = goalXZ;
            }
        }

        internal void OnDestroy()
        {
            Agent = null;
        }
    }
}