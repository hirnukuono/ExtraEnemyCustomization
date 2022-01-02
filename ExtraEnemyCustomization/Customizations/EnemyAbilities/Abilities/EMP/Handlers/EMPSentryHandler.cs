using AK;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers
{
    public class EMPSentryHandler : EMPHandlerBase
    {
        static Color _offColor = new() { r = 0, g = 0, b = 0, a = 0 };
        SentryGunInstance _sentry;
        SentryGunInstance_ScannerVisuals_Plane _visuals;

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            _sentry = gameObject.GetComponent<SentryGunInstance>();
            _visuals = gameObject.GetComponent<SentryGunInstance_ScannerVisuals_Plane>();
            if (_sentry == null || _visuals == null)
            {
                Logger.Error("Missing components on Sentry! Has Sentry?: {0}, Has Visuals?: {1}", _sentry == null, _visuals == null);
            }
        }

        protected override void DeviceOff()
        {
            _visuals.m_scannerPlane.SetColor(_offColor);
            _visuals.UpdateLightProps(_offColor, false);
            _sentry.m_isSetup = false;
            _sentry.m_isScanning = false;
            _sentry.m_isFiring = false;
            _sentry.Sound.Post(EVENTS.SENTRYGUN_STOP_ALL_LOOPS);
        }

        protected override void DeviceOn()
        {
            _sentry.m_isSetup = true;
            _sentry.m_visuals.SetVisualStatus(eSentryGunStatus.BootUp);
            _sentry.m_isScanning = false;
            _sentry.m_startScanTimer = Clock.Time + _sentry.m_initialScanDelay;
            _sentry.Sound.Post(EVENTS.SENTRYGUN_LOW_AMMO_WARNING);
        }

        protected override void FlickerDevice()
        {
            int state = UnityEngine.Random.RandomRangeInt(0, 3);
            _sentry.StopFiring();

            switch(state)
            {
                case 0:
                    _visuals.SetVisualStatus(eSentryGunStatus.OutOfAmmo);
                    break;

                case 1:
                    _visuals.SetVisualStatus(eSentryGunStatus.Scanning);
                    break;

                case 2:
                    _visuals.SetVisualStatus(eSentryGunStatus.HasTarget);
                    break;
            }
        }
    }
}
