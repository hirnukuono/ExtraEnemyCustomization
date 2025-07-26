using Agents;
using EEC.Utils.Unity;
using Timer = EEC.Utils.Unity.Timer;
using Enemies;
using SNetwork;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;

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
        public bool DoGlobalFallback { get; set; } = false;

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
        private AIGraph.AIG_CourseNode _fallbackNode = null!;
        private UnityEngine.Vector3 _fallbackPos;
        private UnityEngine.Quaternion _fallbackRot;

        private IEnumerator? _updateLoop = null;
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
            _fallbackNode = Agent.CourseNode;
            _fallbackPos = Agent.Position;
            _fallbackRot = Agent.Rotation;

            if (Ability.DoGlobalFallback)
                CoroutineManager.StartCoroutine(DoSpawnRoutine().WrapToIl2Cpp());
            else
                _updateLoop = DoSpawnRoutine();
            _stateTimer.Reset(Ability.Delay);
        }

        protected override void OnUpdate()
        {
            _updateLoop?.MoveNext();
        }

        private IEnumerator DoSpawnRoutine()
        {
            while (!_stateTimer.TickAndCheckDone())
                yield return null;

            _stateTimer.Reset(0.0f);
            while (_remainingSpawn > 0)
            {
                while (!_stateTimer.TickAndCheckDone())
                    yield return null;

                if (SNet.IsMaster)
                    SpawnEnemy(Ability.CountPerSpawn);

                _stateTimer.Reset(Ability.DelayPerSpawn);
            }

            DoExit();
            _updateLoop = null;
        }

        private void SpawnEnemy(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (_remainingSpawn > 0)
                {
                    var node = !Ability.DoGlobalFallback || Agent != null ? Agent!.CourseNode : _fallbackNode;
                    var position = !Ability.DoGlobalFallback || Agent != null ? Agent!.Position : _fallbackPos;
                    var rotation = !Ability.DoGlobalFallback || Agent != null ? Agent!.Rotation : _fallbackRot;
                    _ = EnemyAllocator.Current.SpawnEnemy(Ability.EnemyID, node, Ability.AgentMode, position, rotation, null);
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
            if (Ability.StopAgent && (!Ability.DoGlobalFallback || Agent != null))
                StandStill = false;
        }
    }
}