using Agents;
using EEC.Utils.Unity;
using Timer = EEC.Utils.Unity.Timer;
using Enemies;
using SNetwork;

namespace EEC.EnemyCustomizations.EnemyAbilities.Abilities
{
    public sealed class SpawnEnemyAbility : AbilityBase<SpawnEnemyBehaviour>
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

    public sealed class SpawnEnemyBehaviour : AbilityBehaviour<SpawnEnemyAbility>
    {
        private int _remainingSpawn = 0;

        private State _state;
        private Timer _stateTimer;

        public override bool RunUpdateOnlyWhileExecuting => true;
        public override bool AllowEABAbilityWhileExecuting => false;
        public override bool IsHostOnlyBehaviour => false;

        protected override void OnEnter()
        {
            _remainingSpawn = Ability.TotalCount;

            if (_remainingSpawn <= 0)
            {
                DoExit();
                return;
            }

            StandStill = Ability.StopAgent;

            _state = State.StartDelay;
            _stateTimer.Reset(Ability.Delay);
        }

        protected override void OnUpdate()
        {
            switch (_state)
            {
                case State.StartDelay:
                    if (_stateTimer.TickAndCheckDone())
                    {
                        _state = State.Spawning;
                        _stateTimer.Reset(0.0f);
                    }
                    break;

                case State.Spawning:
                    if (_stateTimer.TickAndCheckDone())
                    {
                        if (_remainingSpawn > 0)
                        {
                            if (SNet.IsMaster)
                                SpawnEnemy(Ability.CountPerSpawn);

                            _stateTimer.Reset(Ability.DelayPerSpawn);
                        }
                        else
                        {
                            _state = State.DoneSpawning;
                            _stateTimer.Reset(0.0f);
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
                    _ = EnemyAllocator.Current.SpawnEnemy(Ability.EnemyID, Agent.CourseNode, Ability.AgentMode, position, rotation, null);
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
            if (Ability.StopAgent)
                StandStill = false;
        }

        public enum State
        {
            StartDelay,
            Spawning,
            DoneSpawning
        }
    }
}