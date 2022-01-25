using Agents;
using EECustom.Attributes;
using EECustom.Events;
using EECustom.Extensions;
using Enemies;
using SNetwork;
using UnhollowerBaseLib.Attributes;
using UnityEngine;

namespace EECustom.Customizations.Abilities.Handlers
{
    [InjectToIl2Cpp]
    public class HealthRegenHandler : MonoBehaviour
    {
        public Dam_EnemyDamageBase DamageBase;

        public HealthRegenData RegenData;

        private float _regenInitialTimer = 0.0f;
        private float _regenIntervalTimer = 0.0f;
        private bool _isRegening = false;
        private bool _isInitialTimerDone = false;
        private bool _alwaysRegen = false;
        private bool _isDecay = false;
        private float _regenCapAbsValue = 0.0f;
        private float _regenAmountAbsValue = 0.0f;

        internal void Start()
        {
            _isRegening = false;
            _alwaysRegen = !RegenData.CanDamageInterruptRegen;

            if (!_alwaysRegen)
            {
                EnemyDamageEvents.Damage += OnTakeDamage;
                DamageBase.Owner.AddOnDeadOnce(() =>
                {
                    EnemyDamageEvents.Damage -= OnTakeDamage;
                });
            }

            _regenCapAbsValue = RegenData.RegenCap.GetAbsValue(DamageBase.HealthMax);
            _regenAmountAbsValue = RegenData.RegenAmount.GetAbsValue(DamageBase.HealthMax);

            if (_regenAmountAbsValue <= 0.0f)
                _isDecay = true;

            if (_alwaysRegen || _isDecay)
            {
                OnTakeDamage(DamageBase.Owner, null, 0.0f);
            }
        }

        internal void Update()
        {
            if (!SNet.IsMaster)
                return;

            if (!_isRegening)
                return;

            if (!_isInitialTimerDone && _regenInitialTimer <= Clock.Time)
            {
                _isInitialTimerDone = true;
            }
            else if (_isInitialTimerDone && _regenIntervalTimer <= Clock.Time)
            {
                if (!_isDecay && DamageBase.Health >= _regenCapAbsValue)
                    return;

                if (_isDecay && DamageBase.Health <= _regenCapAbsValue)
                    return;

                var newHealth = DamageBase.Health + _regenAmountAbsValue;
                if (!_isDecay && newHealth >= _regenCapAbsValue)
                {
                    newHealth = _regenCapAbsValue;
                    if (!_alwaysRegen)
                    {
                        _isRegening = false;
                    }
                }
                else if (_isDecay && newHealth <= _regenCapAbsValue)
                {
                    newHealth = _regenCapAbsValue;
                    if (!_alwaysRegen)
                    {
                        _isRegening = false;
                    }
                }

                DamageBase.SendSetHealth(newHealth);

                if (newHealth <= 0.0f)
                {
                    DamageBase.MeleeDamage(DamageBase.HealthMax, null, base.transform.position, Vector3.up, 0);
                }

                _regenIntervalTimer = Clock.Time + RegenData.RegenInterval;
            }
        }

        [HideFromIl2Cpp]
        private void OnTakeDamage(EnemyAgent enemy, Agent inflictor, float damage)
        {
            if (enemy.GlobalID != DamageBase.Owner.GlobalID)
                return;

            _regenInitialTimer = Clock.Time + RegenData.DelayUntilRegenStart;
            _isRegening = true;
            _isInitialTimerDone = false;
        }
    }
}