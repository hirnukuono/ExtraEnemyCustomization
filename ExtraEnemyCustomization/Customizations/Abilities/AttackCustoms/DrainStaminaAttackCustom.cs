﻿using EECustom.Customizations.Shared;
using Enemies;
using Player;

namespace EECustom.Customizations.Abilities
{
    public sealed class DrainStaminaAttackCustom : AttackCustomBase<DrainStaminaSetting>
    {
        public override string GetProcessName()
        {
            return "DrainStamina";
        }

        protected override void OnApplyEffect(DrainStaminaSetting setting, PlayerAgent player, EnemyAgent inflicator)
        {
            setting.DoDrain(player);
        }
    }
}