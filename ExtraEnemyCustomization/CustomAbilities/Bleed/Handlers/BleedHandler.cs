using EECustom.Attributes;
using EECustom.CustomAbilities.Bleed.Inject;
using Player;
using System;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.CustomAbilities.Bleed.Handlers
{
    [InjectToIl2Cpp]
    public sealed class BleedHandler : MonoBehaviour
    {
        public PlayerAgent Agent;

        private float _damage;
        private float _interval;
        private float _bleedingTimer;
        private float _bleedingIntervalTimer;
        private bool _hasLiquid = false;
        private ScreenLiquidSettingName _liquid;
        private bool _bleeding = false;

        internal void Update()
        {
            if (!_bleeding)
                return;

            if (_bleedingTimer <= Clock.Time)
            {
                StopBleed();
                return;
            }

            if (_bleedingIntervalTimer <= Clock.Time)
            {
                if (!Agent.Alive)
                    return;

                if (_hasLiquid)
                {
                    ScreenLiquidManager.DirectApply(_liquid, new Vector2(UnityEngine.Random.Range(0.3f, 0.7f), UnityEngine.Random.Range(0.3f, 0.7f)), Vector2.down);
                }

                Agent.Damage.FireDamage(_damage, Agent);
                _bleedingIntervalTimer = Clock.Time + _interval;
            }
        }

        internal void OnDestroy()
        {
            StopBleed();

            Agent = null;
        }

        [HideFromIl2Cpp]
        public void DoBleed(float damage, float interval, float duration, ScreenLiquidSettingName liquid)
        {
            if (!Agent.Alive)
                return;

            _damage = damage;
            _interval = interval;
            _bleedingTimer = Clock.Time + duration;
            _liquid = liquid;
            _hasLiquid = Enum.IsDefined(typeof(ScreenLiquidSettingName), _liquid);
            _bleeding = true;

            Inject_PUI_LocalPlayerStatus.IsBleeding = true;
        }

        [HideFromIl2Cpp]
        public void StopBleed()
        {
            _bleeding = false;

            Inject_PUI_LocalPlayerStatus.IsBleeding = false;
            GuiManager.PlayerLayer.UpdateHealth(Agent.Damage.GetHealthRel(), Agent.MeleeBuffTimer > Clock.Time);
        }
    }
}