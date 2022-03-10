using EECustom.Customizations.EnemyAbilities;
using EECustom.Customizations.EnemyAbilities.Abilities;
using System;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public sealed class EnemyAbilityCustomConfig : CustomizationConfig
    {
        public BehaviourAbilityCustom[] BehaviourAbilityCustom { get; set; } = Array.Empty<BehaviourAbilityCustom>();
        public DeathAbilityCustom[] DeathAbilityCustom { get; set; } = Array.Empty<DeathAbilityCustom>();
        public EnemyAbilitiesSetting Abilities { get; set; } = new EnemyAbilitiesSetting();

        public override string FileName => "EnemyAbility";
        public override CustomizationConfigType Type => CustomizationConfigType.EnemyAbility;

        public override void Loaded()
        {
            Abilities.RegisterAll();
            EnemyAbilityManager.Setup();
        }

        public override void Unloaded()
        {
            EnemyAbilityManager.Clear();
        }
    }

    public class EnemyAbilitiesSetting
    {
        public FogSphereAbility[] FogSphere { get; set; } = Array.Empty<FogSphereAbility>();
        public ExplosionAbility[] Explosion { get; set; } = Array.Empty<ExplosionAbility>();
        public SpawnEnemyAbility[] SpawnEnemy { get; set; } = Array.Empty<SpawnEnemyAbility>();
        public SpawnWaveAbility[] SpawnWave { get; set; } = Array.Empty<SpawnWaveAbility>();
        public DoAnimAbility[] DoAnim { get; set; } = Array.Empty<DoAnimAbility>();
        public EMPAbility[] EMP { get; set; } = Array.Empty<EMPAbility>();
        public CloakAbility[] Cloak { get; set; } = Array.Empty<CloakAbility>();
        public ChainedAbility[] Chain { get; set; } = Array.Empty<ChainedAbility>();

        public void RegisterAll()
        {
            var list = new List<IAbility>();
            list.AddRange(FogSphere);
            list.AddRange(Explosion);
            list.AddRange(SpawnEnemy);
            list.AddRange(SpawnWave);
            list.AddRange(DoAnim);
            list.AddRange(EMP);
            list.AddRange(Cloak);
            list.AddRange(Chain);

            foreach (var ab in list)
            {
                if (string.IsNullOrEmpty(ab.Name))
                {
                    Logger.Warning("Ignoring EnemyAbility without any Name!");
                    continue;
                }

                EnemyAbilityManager.AddAbility(ab);
            }
        }
    }
}