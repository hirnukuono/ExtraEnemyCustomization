using AK;
using Gear;
using UnityEngine;

namespace EECustom.CustomAbilities.EMP.Handlers
{
    public class EMPBioTrackerHandler : EMPHandlerBase
    {
        private EnemyScanner _scanner;

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            if (!gameObject.TryGetComp(out _scanner))
            {
                Logger.Error("Couldn't get bio-tracker component!");
            }
        }

        protected override void DeviceOff()
        {
            _scanner.Sound.Post(EVENTS.BIOTRACKER_TOOL_LOOP_STOP, true);
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