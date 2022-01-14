using EECustom.Customizations.Abilities.Inject;
using EECustom.Utils;
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
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var setting = EnemyProperty<DoorBreakerSetting>.RegisterOrGet(agent);
            setting.Config = this;
            setting.Damage = Damage;
            setting.UseGlobalTimer = UseGlobalTimer;
            setting.MinDelay = MinDelay;
            setting.MaxDelay = MaxDelay;
        }
    }

    public class DoorBreakerSetting
    {
        public DoorBreakerCustom Config;
        public bool UseGlobalTimer = false;
        public float Damage = 1.0f;
        public float MinDelay = 0.5f;
        public float MaxDelay = 0.75f;
        public float Timer = 0.0f;
    }
}