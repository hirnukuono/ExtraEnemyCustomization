using Agents;
using EECustom.Events;
using EECustom.Utils;
using EECustom.Utils.JsonElements;
using Player;

namespace EECustom.Customizations.Abilities
{
    public class InfectionAttackCustom : EnemyCustomBase
    {
        public InfectionAttackData MeleeData { get; set; } = new();
        public InfectionAttackData TentacleData { get; set; } = new();

        public override string GetProcessName()
        {
            return "InfectionAttack";
        }

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.MeleeDamage += OnMelee;
            LocalPlayerDamageEvents.TentacleDamage += OnTentacle;
        }

        public override void OnConfigUnloaded()
        {
            LocalPlayerDamageEvents.MeleeDamage -= OnMelee;
            LocalPlayerDamageEvents.TentacleDamage -= OnTentacle;
        }

        public void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                ApplyInfection(MeleeData, player, inflictor);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                ApplyInfection(TentacleData, player, inflictor);
            }
        }

        public void ApplyInfection(InfectionAttackData data, PlayerAgent player, Agent _)
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
    }

    public class InfectionAttackData
    {
        public ValueBase Infection { get; set; } = ValueBase.Zero;
        public uint SoundEventID { get; set; } = 0u;
        public bool UseEffect { get; set; } = false;
        public float ScreenLiquidRange { get; set; } = 0.0f;
    }
}