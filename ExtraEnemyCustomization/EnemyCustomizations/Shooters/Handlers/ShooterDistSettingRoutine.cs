using EEC.Utils.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EEC.EnemyCustomizations.Shooters.Handlers
{
    public sealed class ShooterDistSettingRoutine
    {
        public ShooterFireOption DefaultValue;
        public EAB_ProjectileShooter EAB_Shooter;
        public ShooterFireCustom.FireSetting[] FireSettings;

        private ShooterFireCustom.FireSetting _currentSetting = null;
        private static readonly WaitForSeconds _yielder = WaitFor.Seconds[0.125f];

        public IEnumerator Routine()
        {
            while (true)
            {
                yield return _yielder;

                if (EAB_Shooter.m_owner.Locomotion.CurrentStateEnum == Enemies.ES_StateEnum.ShooterAttack)
                    continue;

                if (!EAB_Shooter.m_owner.AI.IsTargetValid)
                    continue;

                var distance = EAB_Shooter.m_owner.AI.Target.m_distance;
                var newSetting = FireSettings.FirstOrDefault(x => x.FromDistance <= distance);
                if (newSetting != _currentSetting)
                {
                    newSetting.ApplyToEAB(EAB_Shooter, DefaultValue);
                    _currentSetting = newSetting;
                }
            }
        }
    }
}
