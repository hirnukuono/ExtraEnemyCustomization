using EECustom.Customizations.Abilities.Handlers;
using EECustom.Customizations.Shared;
using EECustom.Events;
using Enemies;
using UnityEngine;

namespace EECustom.Customizations.Abilities
{
    public class ScoutScreamingCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        private readonly static Color DefaultChargeupColor = ES_ScoutScream.s_screamChargeupColor;

        public Color ChargeupColor { get; set; } = new Color(0f, 1f, 0.8f, 1f) * 2f;
        public Color FogColor { get; set; } = new Color(0f, 1f, 0.8f, 1f) * 20f;
        public float FogIntensity { get; set; } = 1.0f;
        public EffectVolumeSetting EffectVolume { get; set; } = new EffectVolumeSetting();

        public override string GetProcessName()
        {
            return "ScoutScreaming";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            EnemyGlowEvents.RegisterOnGlow(agent, OnGlow);

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

        public Color OnGlow(EnemyAgent agent, Color color, Vector4 location)
        {
            if (color == DefaultChargeupColor)
            {
                return ChargeupColor;
            }

            return color;
        }
    }
}