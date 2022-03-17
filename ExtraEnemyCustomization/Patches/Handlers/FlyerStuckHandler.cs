using Agents;
using EEC.Attributes;
using EEC.Managers;
using EEC.Utils.Unity;
using Enemies;
using SNetwork;
using UnityEngine;

namespace EEC.Patches.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class FlyerStuckHandler : MonoBehaviour
    {
        public EnemyAgent Agent;
        public float UpdateInterval = float.MaxValue;
        public int RetryCount = int.MaxValue;

        private Vector3 _firstPosition;
        private Vector2 _lastGoalXZ;
        private Timer _timer;
        private int _tryCount = -1;
        private bool _shouldCheck = true;

        private void Start()
        {
            if (!SNet.IsMaster)
            {
                enabled = false;
                return;
            }

            if (!gameObject.TryGetComp(out Agent))
            {
                enabled = false;
                return;
            }

            if (!Agent.EnemyBehaviorData.IsFlyer)
            {
                enabled = false;
                return;
            }

            UpdateInterval = ConfigManager.Global.FlyerStuck_Interval;
            RetryCount = ConfigManager.Global.FlyerStuck_Retry;
        }

        private void FixedUpdate()
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

        private void OnDestroy()
        {
            Agent = null;
        }
    }
}