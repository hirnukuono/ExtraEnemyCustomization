using EECustom.Customizations.Abilities.Inject;
using EECustom.Events;
using Enemies;

namespace EECustom.Customizations.Abilities
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
        }

        public override void OnConfigUnloaded()
        {
            LevelEvents.LevelCleanup -= LevelCleanup;
        }

        private void LevelCleanup()
        {
            EB_InCombat_MoveToNextNode_DestroyDoor.s_globalRetryTimer = 0.0f;
            _globalTimer = 0.0f;
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