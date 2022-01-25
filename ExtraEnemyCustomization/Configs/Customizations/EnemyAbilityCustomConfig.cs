using EECustom.Customizations;
using EECustom.Customizations.EnemyAbilities;
using EECustom.Customizations.EnemyAbilities.Abilities;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public sealed class EnemyAbilityCustomConfig : CustomizationConfig
    {
        public BehaviourAbilityCustom[] BehaviourAbilityCustom { get; set; } = new BehaviourAbilityCustom[0];
        public DeathAbilityCustom[] DeathAbilityCustom { get; set; } = new DeathAbilityCustom[0];
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
        public DoAnimAbility[] DoAnim { get; set; } = new DoAnimAbility[0];
        public EMPAbility[] EMP { get; set; } = new EMPAbility[0];

        public void RegisterAll()
        {
            var list = new List<IAbility>();
            list.AddRange(FogSphere);
            list.AddRange(Explosion);
            list.AddRange(SpawnEnemy);
            list.AddRange(DoAnim);
            list.AddRange(EMP);

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