using Agents;
using AK;
using EECustom.Events;
using Enemies;
using SNetwork;
using System.Collections.Generic;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class SpawnEnemyAbility : AbilityBase<SpawnEnemyBehaviour>
    {
        public bool StopAgent { get; set; } = false;
        public float Delay { get; set; } = 0.0f;
        public uint EnemyID { get; set; } = 0u;
        public AgentMode AgentMode { get; set; } = AgentMode.Agressive;
        public int TotalCount { get; set; } = 0;
        public int CountPerSpawn { get; set; } = 0;
        public float DelayPerSpawn { get; set; } = 0.0f;

        public override void OnAbilityLoaded()
        {
            if (TotalCount < 0)
            {
                LogError($"TotalCount was below zero! : setting to default value");
                TotalCount = 0;
            }

            if (CountPerSpawn < 1)
            {
                LogError($"CountPerSpawn was below one! : setting to default value");
                CountPerSpawn = 1;
            }
        }
    }

    public class SpawnEnemyBehaviour : AbilityBehaviour<SpawnEnemyAbility>
    {
        private int _remainingSpawn = 0;

        private bool _shouldRevertNavMeshAgent = false;
        private State _state;
        private float _stateTimer = 0.0f;
        private readonly List<ushort> _spawnedChilds = new();

        public override bool AllowEABAbilityWhileExecuting => false;
        public override bool IsHostOnlyBehaviour => false;

        protected override void OnSetup()
        {
            EnemyEvents.Spawned += OnEnemySpawned;
        }

        protected override void OnDead()
        {
            EnemyEvents.Spawned -= OnEnemySpawned;
        }

        private void OnEnemySpawned(EnemyAgent agent)
        {
            var index = _spawnedChilds.IndexOf(agent.GlobalID);
            if (index > -1)
            {
                _spawnedChilds.RemoveAt(index);
                agent.Sound.Post(agent.EnemySFXData.SFX_ID_hibernateWakeUp);
                agent.Sound.Post(EVENTS.BIRTHER_BABY_DROP);
            }
        }

        protected override void OnEnter()
        {
            _remainingSpawn = Ability.TotalCount;
            _shouldRevertNavMeshAgent = false;

            if (_remainingSpawn <= 0)
            {
                DoExit();
                return;
            }

            if (Ability.StopAgent && Agent.AI.m_navMeshAgent.enabled)
            {
                Agent.AI.m_navMeshAgent.isStopped = true;
                _shouldRevertNavMeshAgent = true;
            }

            _state = State.StartDelay;
            _stateTimer = Clock.Time + Ability.Delay;
        }

        protected override void OnUpdate()
        {
            switch (_state)
            {
                case State.StartDelay:
                    if (_stateTimer <= Clock.Time)
                    {
                        _state = State.Spawning;
                        _stateTimer = 0.0f;
                    }
                    else
                    {
                        Agent.Voice.PlayVoiceEvent(EVENTS.BIRTHER_GOING_INTO_LABOR);
                    }
                    break;

                case State.Spawning:
                    if (_stateTimer <= Clock.Time)
                    {
                        if (_remainingSpawn > 0)
                        {
                            if (SNet.IsMaster)
                                SpawnEnemy(Ability.CountPerSpawn);

                            _stateTimer = Clock.Time + Ability.DelayPerSpawn;
                        }
                        else
                        {
                            _state = State.DoneSpawning;
                            _stateTimer = 0.0f;
                        }
                    }
                    break;

                case State.DoneSpawning:
                    DoExit();
                    break;

            }
        }

        private void SpawnEnemy(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_remainingSpawn > 0)
                {
                    var position = Agent.Position;
                    var rotation = Agent.Rotation;
                    var enemyAgent = EnemyAllocator.Current.SpawnEnemy(Ability.EnemyID, Agent.CourseNode, Ability.AgentMode, position, rotation, null);
                    _spawnedChilds.Add(enemyAgent.GlobalID);
                    Logger.Error($"key was : {enemyAgent.GlobalID}");
                    _remainingSpawn--;
                }
                else
                {
                    break;
                }
            }
        }

        protected override void OnExit()
        {
            if (_shouldRevertNavMeshAgent)
            {
                Agent.AI.m_navMeshAgent.isStopped = false;
            }
        }

        public enum State
        {
            StartDelay,
            Spawning,
            DoneSpawning
        }
    }
}
