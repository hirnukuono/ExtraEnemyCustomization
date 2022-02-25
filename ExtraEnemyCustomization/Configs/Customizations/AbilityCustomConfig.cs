using EECustom.Customizations;
using EECustom.Customizations.Abilities;
using System;
using System.Collections.Generic;

namespace EECustom.Configs.Customizations
{
    public sealed class AbilityCustomConfig : CustomizationConfig
    {
        public BirthingCustom[] BirthingCustom { get; set; } = Array.Empty<BirthingCustom>();
        public FogSphereCustom[] FogSphereCustom { get; set; } = Array.Empty<FogSphereCustom>();
        public HealthRegenCustom[] HealthRegenCustom { get; set; } = Array.Empty<HealthRegenCustom>();
        public InfectionAttackCustom[] InfectionAttackCustom { get; set; } = Array.Empty<InfectionAttackCustom>();
        public KnockbackAttackCustom[] KnockbackAttackCustom { get; set; } = Array.Empty<KnockbackAttackCustom>();
        public ExplosiveAttackCustom[] ExplosiveAttackCustom { get; set; } = Array.Empty<ExplosiveAttackCustom>();
        public BleedAttackCustom[] BleedAttackCustom { get; set; } = Array.Empty<BleedAttackCustom>();
        public DrainStaminaAttackCustom[] DrainStaminaAttackCustom { get; set; } = Array.Empty<DrainStaminaAttackCustom>();
        public DoorBreakerCustom[] DoorBreakerCustom { get; set; } = Array.Empty<DoorBreakerCustom>();
        public ScoutScreamingCustom[] ScoutScreamingCustom { get; set; } = Array.Empty<ScoutScreamingCustom>();

        public override string FileName => "Ability";
        public override CustomizationConfigType Type => CustomizationConfigType.Ability;
    }
}