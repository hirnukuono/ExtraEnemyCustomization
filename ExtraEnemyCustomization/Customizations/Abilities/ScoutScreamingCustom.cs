using EECustom.Customizations.Abilities.Handlers;
using EECustom.Customizations.Shared;
using Enemies;
using UnityEngine;

namespace EECustom.Customizations.Abilities
{
    public sealed class ScoutScreamingCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyGlowEvent
    {
        public readonly static Color DefaultChargeupColor = ES_ScoutScream.s_screamChargeupColor;
        public readonly static Color DefaultScreamColor = ES_ScoutScream.s_screamPopColor;

        public Color ChargeupColor { get; set; } = new Color(0f, 1f, 0.8f, 1f) * 2f;
        public Color ScreamColor { get; set; } = new Color(0f, 1f, 0.8f, 1f) * 20f;
        public Color FogColor { get; set; } = new Color(0f, 1f, 0.8f, 1f) * 20f;
        public float FogIntensity { get; set; } = 1.0f;
        public EffectVolumeSetting EffectVolume { get; set; } = new EffectVolumeSetting();

        public override string GetProcessName()
        {
            return "ScoutScreaming";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var handler = agent.gameObject.AddComponent<ScoutFogSphereHandler>();
            handler.ScoutScream = agent.Locomotion.ScoutScream;
            handler.FogColor = FogColor;
            handler.FogIntensity = FogIntensity;

            if (EffectVolume.Enabled)
            {
                handler.EVSphere = EffectVolume.CreateSphere(agent.transform.position, 0.0f, 0.0f);
                EffectVolumeManager.RegisterVolume(handler.EVSphere);
            }
        }

        public bool OnGlow(EnemyAgent agent, ref GlowInfo glowInfo)
        {
            if (glowInfo.Color == DefaultChargeupColor)
            {
                glowInfo = glowInfo.ChangeColor(ChargeupColor);
                return true;
            }
            else if (glowInfo.Color == DefaultScreamColor)
            {
                glowInfo = glowInfo.ChangeColor(ScreamColor);
                return true;
            }

            return false;
        }
    }
}