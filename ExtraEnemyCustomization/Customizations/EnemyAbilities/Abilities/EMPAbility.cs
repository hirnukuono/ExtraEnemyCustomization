using Enemies;
using System;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities
{
    public class EMPAbility : AbilityBase<EMPBehaviour>
    {
        public Color FogColor { get; set; } = new Color(0.525f, 0.956f, 0.886f, 1.0f);
        public float FogIntensity { get; set; } = 1.0f;
        public Color BuildupColor { get; set; } = new Color(0.525f, 0.956f, 0.886f, 1.0f) * 2.0f;
        public Color ScreamColor { get; set; } = new Color(0.525f, 0.956f, 0.886f, 1.0f) * 20.0f;

        public override void OnBehaviourAssigned(EnemyAgent agent, EMPBehaviour behaviour)
        {
            behaviour.FogColor = FogColor;
            behaviour.FogIntensity = FogIntensity;
            behaviour.BuildupColor = BuildupColor;
            behaviour.ScreamColor = ScreamColor;
        }
    }

    //MAJOR: Please Implement this Dak
    [Obsolete("PLEASE IMPLEMENT THIS DAK", false)]
    public class EMPBehaviour : AbilityBehaviour
    {
        public Color FogColor;
        public float FogIntensity;
        public Color BuildupColor;
        public Color ScreamColor;

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
