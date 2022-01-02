using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using EECustom.Extensions;
using Gear;
using AK;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers
{
    public class EMPBioTrackerHandler : EMPHandlerBase
    {
        EnemyScanner _scanner;
        public override void Setup(GameObject gameObject, EMPController controller)
        {
            _scanner = gameObject.GetComponent<EnemyScanner>();
            if (_scanner == null)
            {
                Logger.Error("Couldn't get bio-tracker component!");
            }
        }

        protected override void DeviceOff()
        {
            _scanner.m_graphics.m_display.enabled = false;
        }

        protected override void DeviceOn()
        {
            _scanner.m_graphics.m_display.enabled = true;
        }

        protected override void FlickerDevice()
        {
            _scanner.enabled = FlickerUtil();
        }
    }
}
