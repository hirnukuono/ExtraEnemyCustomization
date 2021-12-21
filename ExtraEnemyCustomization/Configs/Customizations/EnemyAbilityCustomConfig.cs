using EECustom.Customizations;
using EECustom.Customizations.EnemyAbilities;
using EECustom.Customizations.EnemyAbilities.Abilities;
using System;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public class EnemyAbilityCustomConfig : CustomizationConfig
    {
        public BehaviourAbilityCustom[] BehaviourAbilityCustom { get; set; } = new BehaviourAbilityCustom[0];
        public DeathAbilityCustom[] DeathAbilityCustom { get; set; } = new DeathAbilityCustom[0];
        public EnemyAbilitiesSetting Abilities { get; set; } = new EnemyAbilitiesSetting();
        public EnemyAbilitiesEvent[] Events { get; set; } = new EnemyAbilitiesEvent[0];

        public override EnemyCustomBase[] GetAllSettings()
        {
            var list = new List<EnemyCustomBase>();
            list.AddRange(BehaviourAbilityCustom);
            list.AddRange(DeathAbilityCustom);
            return list.ToArray();
        }
    }

    public class EnemyAbilitiesSetting
    {
        public ChainedAbility[] Chain { get; set; } = new ChainedAbility[0];
        public FogSphereAbility[] FogSphere { get; set; } = new FogSphereAbility[0];
        public ExplosionAbility[] Explosion { get; set; } = new ExplosionAbility[0];
        public SpawnEnemyAbility[] SpawnEnemy { get; set; } = new SpawnEnemyAbility[0];
        public EMPAbility[] EMP { get; set; } = new EMPAbility[0];

        public void RegisterAll()
        {
            var list = new List<IAbility>();
            list.AddRange(FogSphere);
            list.AddRange(Explosion);
            list.AddRange(SpawnEnemy);
            list.AddRange(EMP);

            foreach (var ab in list)
            {
                EnemyAbilityManager.AddAbility(ab);
            }
        }
    }

    public class EnemyAbilitiesEvent
    {

    }
}
