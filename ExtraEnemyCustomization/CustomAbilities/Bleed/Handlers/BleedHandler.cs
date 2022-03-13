using BepInEx.IL2CPP.Utils;
using EECustom.Attributes;
using EECustom.CustomAbilities.Bleed.Inject;
using EECustom.Utils;
using Player;
using System;
using System.Collections;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.CustomAbilities.Bleed.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class BleedHandler : MonoBehaviour
    {
        public PlayerAgent Agent;

        private bool _globalBleedRunning = false;
        private Coroutine _globalBleedRoutine = null;
        private int _bleedRoutineCounter = 0;

        internal void OnDestroy()
        {
            StopBleed();
            Agent = null;
        }

        [HideFromIl2Cpp]
        public void DoBleed(BleedingData bleedData)
        {
            if (!Agent.Alive)
                return;

            Inject_PUI_LocalPlayerStatus.IsBleeding = true;

            if (bleedData.doStack)
            {
                MonoBehaviourExtensions.StartCoroutine(this, DoStackableBleed(bleedData));
                _bleedRoutineCounter++;
            }
            else
            {
                if (_globalBleedRoutine != null)
                {
                    StopCoroutine(_globalBleedRoutine);
                }

                _globalBleedRoutine = MonoBehaviourExtensions.StartCoroutine(this, DoGlobalBleed(bleedData));
                _globalBleedRunning = true;
            }
        }

        private IEnumerator DoGlobalBleed(BleedingData bleedData)
        {
            var intervalYielder = new WaitForSeconds(bleedData.interval);
            var timer = 0.0f;

            var liquid = bleedData.liquid;
            var hasLiquid = Enum.IsDefined(typeof(ScreenLiquidSettingName), liquid);

            while (timer < bleedData.duration)
            {
                if (Agent.Alive)
                {
                    if (hasLiquid)
                    {
                        ScreenLiquidManager.DirectApply(liquid, new Vector2(Rand.Range(0.3f, 0.7f), Rand.Range(0.3f, 0.7f)), Vector2.down);
                    }
                    Agent.Damage.FireDamage(bleedData.damage, Agent);
                }

                timer += bleedData.interval;
                yield return intervalYielder;
            }

            _globalBleedRunning = false;
            _globalBleedRoutine = null;

            Inject_PUI_LocalPlayerStatus.IsBleeding = _globalBleedRunning || _bleedRoutineCounter > 0;
            GuiManager.PlayerLayer.UpdateHealth(Agent.Damage.GetHealthRel(), Agent.MeleeBuffTimer > Clock.Time);
        }

        private IEnumerator DoStackableBleed(BleedingData bleedData)
        {
            var intervalYielder = new WaitForSeconds(bleedData.interval);
            var timer = 0.0f;

            var liquid = bleedData.liquid;
            var hasLiquid = Enum.IsDefined(typeof(ScreenLiquidSettingName), liquid);

            while (timer < bleedData.duration)
            {
                if (Agent.Alive)
                {
                    if (hasLiquid)
                    {
                        ScreenLiquidManager.DirectApply(liquid, new Vector2(Rand.Range(0.3f, 0.7f), Rand.Range(0.3f, 0.7f)), Vector2.down);
                    }
                    Agent.Damage.FireDamage(bleedData.damage, Agent);
                }

                timer += bleedData.interval;
                yield return intervalYielder;
            }
            _bleedRoutineCounter--;

            Inject_PUI_LocalPlayerStatus.IsBleeding = _globalBleedRunning || _bleedRoutineCounter > 0;
            GuiManager.PlayerLayer.UpdateHealth(Agent.Damage.GetHealthRel(), Agent.MeleeBuffTimer > Clock.Time);
        }

        [HideFromIl2Cpp]
        public void StopBleed()
        {
            StopAllCoroutines();
            _bleedRoutineCounter = 0;
            _globalBleedRunning = false;
            _globalBleedRoutine = null;

            Inject_PUI_LocalPlayerStatus.IsBleeding = false;
            GuiManager.PlayerLayer.UpdateHealth(Agent.Damage.GetHealthRel(), Agent.MeleeBuffTimer > Clock.Time);
        }
    }
}