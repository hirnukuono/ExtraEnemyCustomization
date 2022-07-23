using EEC.EnemyCustomizations.Abilities.Inject;
using EEC.Events;
using EEC.Networking;
using EEC.Utils;
using Enemies;
using SNetwork;

namespace EEC.EnemyCustomizations.Abilities
{
    public sealed class DoorBreakerCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public bool UseGlobalTimer { get; set; } = false;
        public float Damage { get; set; } = 1.0f;
        public float MinDelay { get; set; } = 0.5f;
        public float MaxDelay { get; set; } = 0.75f;

        internal float _globalTimer = 0.0f;

        public override string GetProcessName()
        {
            return "DoorBreaker";
        }

        public override void OnConfigLoaded()
        {
            EB_InCombat_MoveToNextNode_DestroyDoor.s_globalRetryTimer = float.MaxValue;
            Inject_EB_DestroyDoor.ShouldOverride = true;

            LevelEvents.LevelCleanup += LevelCleanup;
            SNetEvents.PrepareRecall += RecallDone;
        }

        public override void OnConfigUnloaded()
        {
            EB_InCombat_MoveToNextNode_DestroyDoor.s_globalRetryTimer = 0.0f;
            Inject_EB_DestroyDoor.ShouldOverride = false;

            LevelEvents.LevelCleanup -= LevelCleanup;
            SNetEvents.RecallComplete -= RecallDone;
        }

        private void LevelCleanup()
        {
            _globalTimer = 0.0f;
            Inject_EB_DestroyDoor.GlobalTimer = 0.0f;
        }

        private void RecallDone(eBufferType _)
        {
            var properties = EnemyProperty<DoorBreakerProperty>.Properties;
            foreach (var property in properties)
            {
                property.Timer = 0.0f;
            }
            _globalTimer = 0.0f;
            Inject_EB_DestroyDoor.GlobalTimer = 0.0f;
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var setting = agent.RegisterOrGetProperty<DoorBreakerProperty>();
            setting.Config = this;
            setting.Damage = Damage;
            setting.UseGlobalTimer = UseGlobalTimer;
            setting.MinDelay = MinDelay;
            setting.MaxDelay = MaxDelay;
        }
    }

    internal sealed class DoorBreakerProperty
    {
        public DoorBreakerCustom Config;
        public bool UseGlobalTimer = false;
        public float Damage = 1.0f;
        public float MinDelay = 0.5f;
        public float MaxDelay = 0.75f;
        public float Timer = 0.0f;
    }
}