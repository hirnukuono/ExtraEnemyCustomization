using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers
{
    public class EMPPlayerHudHandler : EMPHandlerBase
    {
        readonly List<RectTransformComp> _targets = new();

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            _targets.Add(GuiManager.PlayerLayer.m_compass);
            _targets.Add(GuiManager.PlayerLayer.m_wardenObjective);
            _targets.Add(GuiManager.PlayerLayer.Inventory);
            _targets.Add(GuiManager.PlayerLayer.m_playerStatus);
        }

        protected override void DeviceOff()
        {
            foreach (var target in _targets)
            {
                target.SetVisible(false);
            }
        }

        protected override void DeviceOn()
        {
            foreach (var target in _targets)
            {
                target.SetVisible(true);
            }
        }

        protected override void FlickerDevice()
        {
            foreach (var target in _targets)
            {
                int isEnabled = UnityEngine.Random.RandomRangeInt(0, 2);

                target.SetVisible(isEnabled == 1);
            }
        }
    }
}
