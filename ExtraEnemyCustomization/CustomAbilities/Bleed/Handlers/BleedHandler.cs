using EEC.Attributes;
using EEC.CustomAbilities.Bleed.Inject;
using EEC.Utils;
using Player;
using System;
using System.Collections;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EEC.CustomAbilities.Bleed.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class BleedHandler : MonoBehaviour
    {
        public PlayerAgent Agent;

        private bool _globalBleedRunning = false;
        private Coroutine _globalBleedRoutine = null;
        private int _bleedRoutineCounter = 0;
        private uint _specialOverrideText = 0u;

        [HideFromIl2Cpp]
        public void DoBleed(BleedingData bleedData)
        {
            if (!Agent.Alive)
                return;

            Inject_PUI_LocalPlayerStatus.IsBleeding = true;

            if (bleedData.doStack)
            {
                this.StartCoroutine(DoStackableBleed(bleedData));
            }
            else
            {
                if (_globalBleedRoutine != null)
                {
                    StopCoroutine(_globalBleedRoutine);
                }
                _globalBleedRoutine = this.StartCoroutine(DoGlobalBleed(bleedData));
            }
        }

        [HideFromIl2Cpp]
        private IEnumerator DoGlobalBleed(BleedingData bleedData)
        {
            _globalBleedRunning = true;

            var shouldRevertText = false;
            if (_specialOverrideText == 0u)
            {
                _specialOverrideText = bleedData.textSpecialOverride;
                Inject_PUI_LocalPlayerStatus.SpecialOverrideTextID = bleedData.textSpecialOverride;
                shouldRevertText = true;
            }

            var intervalYielder = WaitFor.Seconds[bleedData.interval];
            var timer = 0.0f;

            var liquid = bleedData.liquid;
            var hasLiquid = Enum.IsDefined(typeof(ScreenLiquidSettingName), liquid);

            while (timer < bleedData.duration)
            {
                if (hasLiquid)
                {
                    ScreenLiquidManager.DirectApply(liquid, new Vector2(Rand.Range(0.3f, 0.7f), Rand.Range(0.3f, 0.7f)), Vector2.down);
                }

                if (Agent.Alive)
                {
                    Agent.Damage.FireDamage(bleedData.damage, Agent);
                }

                timer += bleedData.interval;
                yield return intervalYielder;
            }

            _globalBleedRunning = false;
            _globalBleedRoutine = null;

            if (shouldRevertText)
            {
                _specialOverrideText = 0u;
                Inject_PUI_LocalPlayerStatus.SpecialOverrideTextID = 0u;
            }

            Inject_PUI_LocalPlayerStatus.IsBleeding = _globalBleedRunning || _bleedRoutineCounter > 0;
            GuiManager.PlayerLayer.UpdateHealth(Agent.Damage.GetHealthRel(), Agent.MeleeBuffTimer > Clock.Time);
        }

        [HideFromIl2Cpp]
        private IEnumerator DoStackableBleed(BleedingData bleedData)
        {
            _bleedRoutineCounter++;

            var shouldRevertText = false;
            if (_specialOverrideText == 0u)
            {
                _specialOverrideText = bleedData.textSpecialOverride;
                Inject_PUI_LocalPlayerStatus.SpecialOverrideTextID = bleedData.textSpecialOverride;
                shouldRevertText = true;
            }

            var intervalYielder = WaitFor.Seconds[bleedData.interval];
            var timer = 0.0f;

            var liquid = bleedData.liquid;
            var hasLiquid = Enum.IsDefined(typeof(ScreenLiquidSettingName), liquid);

            while (timer < bleedData.duration)
            {
                if (hasLiquid)
                {
                    ScreenLiquidManager.DirectApply(liquid, new Vector2(Rand.Range(0.3f, 0.7f), Rand.Range(0.3f, 0.7f)), Vector2.down);
                }

                if (Agent.Alive)
                {
                    Agent.Damage.FireDamage(bleedData.damage, Agent);
                }

                timer += bleedData.interval;
                yield return intervalYielder;
            }

            _bleedRoutineCounter--;

            if (shouldRevertText)
            {
                _specialOverrideText = 0u;
                Inject_PUI_LocalPlayerStatus.SpecialOverrideTextID = 0u;
            }

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
            Inject_PUI_LocalPlayerStatus.SpecialOverrideTextID = 0u;
            GuiManager.PlayerLayer.UpdateHealth(Agent.Damage.GetHealthRel(), Agent.MeleeBuffTimer > Clock.Time);
        }

        private void OnDestroy()
        {
            StopBleed();
            Agent = null;
        }
    }
}