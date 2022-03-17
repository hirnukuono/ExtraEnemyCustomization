using EECustom.Utils;
using EECustom.Utils.JsonElements;
using Enemies;
using Player;

namespace EECustom.EnemyCustomizations.Abilities
{
    public sealed class InfectionAttackCustom : AttackCustomBase<InfectionAttackCustom.AttackData>
    {
        public override bool DisableProjectileDamageEvent => false;

        public override string GetProcessName()
        {
            return "InfectionAttack";
        }

        protected override void OnApplyEffect(AttackData data, PlayerAgent player, EnemyAgent inflicator)
        {
            var infectionAbs = data.Infection.GetAbsValue(PlayerData.MaxInfection);
            if (infectionAbs == 0.0f)
                return;

            if (data.SoundEventID != 0u)
            {
                player.Sound.Post(data.SoundEventID);
            }

            if (data.UseEffect)
            {
                var liquidSetting = ScreenLiquidSettingName.spitterJizz;
                if (infectionAbs < 0.0f)
                {
                    liquidSetting = ScreenLiquidSettingName.disinfectionStation_Apply;
                }
                ScreenLiquidManager.TryApply(liquidSetting, player.Position, data.ScreenLiquidRange, true);
            }

            player.Damage.ModifyInfection(new pInfection()
            {
                amount = infectionAbs,
                effect = pInfectionEffect.None,
                mode = pInfectionMode.Add
            }, true, true);
        }

        protected override void OnApplyProjectileEffect(AttackData setting, PlayerAgent player, EnemyAgent inflictor, ProjectileBase projectile)
        {
            OnApplyEffect(setting, player, inflictor);
        }

        public sealed class AttackData
        {
            public ValueBase Infection { get; set; } = ValueBase.Zero;
            public uint SoundEventID { get; set; } = 0u;
            public bool UseEffect { get; set; } = false;
            public float ScreenLiquidRange { get; set; } = 0.0f;
        }
    }
}