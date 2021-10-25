using Agents;
using EECustom.Events;
using EECustom.Utils;
using Enemies;
using Player;
using System.Collections.Generic;

namespace EECustom.Customizations.Abilities
{
    public class InfectionAttackCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyDespawnedEvent
    {
        public InfectionAttackData MeleeData { get; set; } = new InfectionAttackData();
        public InfectionAttackData TentacleData { get; set; } = new InfectionAttackData();

        private readonly List<ushort> _EnemyList = new List<ushort>();

        public override string GetProcessName()
        {
            return "InfectionAttack";
        }

        public override void OnConfigLoaded()
        {
            PlayerDamageEvents.OnMeleeDamage += OnMelee;
            PlayerDamageEvents.OnTentacleDamage += OnTentacle;
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var id = agent.GlobalID;
            if (id == ushort.MaxValue)
                return;

            if (!_EnemyList.Contains(id))
            {
                _EnemyList.Add(id);
            }
        }

        public void OnDespawned(EnemyAgent agent)
        {
            _EnemyList.Remove(agent.GlobalID);
        }

        public void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            if (_EnemyList.Contains(inflictor.GlobalID))
            {
                ApplyInfection(MeleeData, player, inflictor);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (_EnemyList.Contains(inflictor.GlobalID))
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