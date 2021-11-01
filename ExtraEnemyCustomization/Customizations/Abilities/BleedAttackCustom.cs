﻿using Agents;
using EECustom.Customizations.Abilities.Handlers;
using EECustom.Events;
using EECustom.Managers;
using EECustom.Utils;
using Enemies;
using Gear;
using Player;
using System.Collections.Generic;

namespace EECustom.Customizations.Abilities
{
    public class BleedAttackCustom : EnemyCustomBase
    {
        public BleedData MeleeData { get; set; } = new();
        public BleedData TentacleData { get; set; } = new();

        private readonly System.Random _Random = new();

        private BleedingHandler _BleedingHandler;

        public override string GetProcessName()
        {
            return "BleedAttack";
        }

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.OnMeleeDamage += OnMelee;
            LocalPlayerDamageEvents.OnTentacleDamage += OnTentacle;
            LevelEvents.OnBuildStart += OnBuildStart;
            LevelEvents.OnLevelCleanup += OnLevelCleanup;

            if (ConfigManager.Current.AbilityCustom.CanMediStopBleeding)
                ResourcePackEvents.OnReceiveMedi += RecieveMedi;
        }

        public void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                var enemyAgent = inflictor.TryCast<EnemyAgent>();
                if (enemyAgent != null)
                    DoBleed(MeleeData);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (IsTarget(inflictor.GlobalID))
            {
                var enemyAgent = inflictor.TryCast<EnemyAgent>();
                if (enemyAgent != null)
                    DoBleed(TentacleData);
            }
        }

        private void DoBleed(BleedData data)
        {
            if (data.ChanceToBleed > _Random.NextDouble())
            {
                _BleedingHandler.DoBleed(data.Damage.GetAbsValue(PlayerData.MaxHealth), data.BleedInterval, data.BleedDuration);
            }
        }

        public void RecieveMedi(iResourcePackReceiver receiver)
        {
            var player = receiver.TryCast<PlayerAgent>();
            if (player != null && player.IsLocallyOwned)
            {
                _BleedingHandler.StopBleed();
            }
        }

        public void OnBuildStart()
        {
            var localPlayer = PlayerManager.GetLocalPlayerAgent();

            _BleedingHandler = localPlayer.gameObject.GetComponent<BleedingHandler>();
            if (_BleedingHandler == null)
            {
                _BleedingHandler = localPlayer.gameObject.AddComponent<BleedingHandler>();
            }
            _BleedingHandler.Agent = localPlayer;
        }

        public void OnLevelCleanup()
        {
            _BleedingHandler.StopBleed();
        }
    }

    public class BleedData
    {
        public ValueBase Damage { get; set; } = ValueBase.Zero;
        public float ChanceToBleed { get; set; } = 0.0f;
        public float BleedInterval { get; set; } = 0.0f;
        public float BleedDuration { get; set; } = 0.0f;
    }
}