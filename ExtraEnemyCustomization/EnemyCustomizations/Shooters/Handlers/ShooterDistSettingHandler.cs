using EEC.Attributes;
using EEC.Utils.Unity;
using System.Linq;
using UnityEngine;

namespace EEC.EnemyCustomizations.Shooters.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class ShooterDistSettingHandler : MonoBehaviour
    {
        public ShooterFireOption DefaultValue;
        public EAB_ProjectileShooter EAB_Shooter;
        public ShooterFireCustom.FireSetting[] FireSettings;

        private ShooterFireCustom.FireSetting _currentSetting = null;
        private Timer _updateTimer = new(0.125f);

        private void Update()
        {
            if (!_updateTimer.TickAndCheckDone())
                return;

            if (EAB_Shooter.m_owner.Locomotion.CurrentStateEnum == Enemies.ES_StateEnum.ShooterAttack)
                return;

            _updateTimer.Reset();

            if (!EAB_Shooter.m_owner.AI.IsTargetValid)
                return;

            var distance = EAB_Shooter.m_owner.AI.Target.m_distance;
            var newSetting = FireSettings.FirstOrDefault(x => x.FromDistance <= distance);
            if (newSetting != _currentSetting)
            {
                newSetting.ApplyToEAB(EAB_Shooter, DefaultValue);
                _currentSetting = newSetting;
            }
        }

        private void OnDestroy()
        {
            EAB_Shooter = null;
            FireSettings = null;
            _currentSetting = null;
        }
    }
}