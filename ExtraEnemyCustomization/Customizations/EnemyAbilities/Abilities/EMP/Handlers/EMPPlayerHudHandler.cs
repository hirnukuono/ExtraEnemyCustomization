using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers
{
    public class EMPPlayerHudHandler : EMPHandlerBase
    {
        private readonly List<RectTransformComp> _targets = new();
        public static EMPPlayerHudHandler Instance => _instance;
        private static EMPPlayerHudHandler _instance;

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            if (_instance != null) return;

            _targets.Add(GuiManager.PlayerLayer.m_compass);
            _targets.Add(GuiManager.PlayerLayer.m_wardenObjective);
            _targets.Add(GuiManager.PlayerLayer.Inventory);
            _targets.Add(GuiManager.PlayerLayer.m_playerStatus);
            _instance = this;
        }

        protected override void DeviceOff()
        {
            foreach (var target in _targets)
            {
                target.gameObject.SetActive(false);
            }
            GuiManager.NavMarkerLayer.SetVisible(false);
            Logger.Debug("Player HUD off");
        }

        protected override void DeviceOn()
        {
            foreach (var target in _targets)
            {
                target.gameObject.SetActive(true);
            }
            GuiManager.NavMarkerLayer.SetVisible(true);
            Logger.Debug("Player HUD on");
        }

        protected override void FlickerDevice()
        {
            foreach (var target in _targets)
            {
                int isEnabled = UnityEngine.Random.RandomRangeInt(0, 2);

                target.SetVisible(isEnabled == 1);
            }

            GuiManager.NavMarkerLayer.SetVisible(UnityEngine.Random.RandomRangeInt(0, 2) == 0);
        }
    }
}