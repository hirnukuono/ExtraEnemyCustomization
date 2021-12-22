using System;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class EMPAbility : AbilityBase<EMPBehaviour>
    {
        public uint ChargeUpSoundId { get; set; } = 0u;
        public uint ActivateSoundId { get; set; } = 0u;
        public int ChargeUpDuration { get; set; } = 5;
        public float  EffectDuration { get; set; } = 30;
        public int EffectRange { get; set; } = 20;
        public bool InvincibleWhileCharging { get; set; } = true;
        public Color FogColor { get; set; } = new Color(0.525f, 0.956f, 0.886f, 1.0f);
        public float FogIntensity { get; set; } = 1.0f;
        public Color BuildupColor { get; set; } = new Color(0.525f, 0.956f, 0.886f, 1.0f) * 2.0f;
        public Color ScreamColor { get; set; } = new Color(0.525f, 0.956f, 0.886f, 1.0f) * 20.0f;
    }

    public class EMPBehaviour : AbilityBehaviour<EMPAbility>
    {
        private float _stateTimer;

        public override bool AllowEABAbilityWhileExecuting => false;
        public override bool IsHostOnlyBehaviour => false;

        protected override void OnEnter()
        {
            base.OnEnter();
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        private enum EMPState
        {
            BuildUp,
            WaveStart,
            LightOffSFX,
            WaveExpand,
            WaveEnd,
            Done
        }
    }
}
