﻿using Agents;
using EECustom.Customizations.Abilities.Handlers;
using EECustom.Events;
using EECustom.Managers;
using EECustom.Utils;
using EECustom.Utils.JsonElements;
using Enemies;
using Gear;
using Player;

namespace EECustom.Customizations.Abilities
{
    public class BleedAttackCustom : EnemyCustomBase
    {
        public BleedData MeleeData { get; set; } = new();
        public BleedData TentacleData { get; set; } = new();

        private readonly System.Random _random = new();

        private BleedingHandler _bleedingHandler;

        public override string GetProcessName()
        {
            return "BleedAttack";
        }

        public override void OnConfigLoaded()
        {
            LocalPlayerDamageEvents.MeleeDamage += OnMelee;
            LocalPlayerDamageEvents.TentacleDamage += OnTentacle;
            LevelEvents.BuildStart += OnBuildStart;
            LevelEvents.LevelCleanup += OnLevelCleanup;

            if (ConfigManager.Current.AbilityCustom.CanMediStopBleeding)
                ResourcePackEvents.ReceiveMedi += RecieveMedi;
        }

        public override void OnConfigUnloaded()
        {
            LocalPlayerDamageEvents.MeleeDamage -= OnMelee;
            LocalPlayerDamageEvents.TentacleDamage -= OnTentacle;

            if (ConfigManager.Current.AbilityCustom.CanMediStopBleeding)
                ResourcePackEvents.ReceiveMedi -= RecieveMedi;
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
            if (data.ChanceToBleed > _random.NextDouble())
            {
                _bleedingHandler.DoBleed(data.Damage.GetAbsValue(PlayerData.MaxHealth), data.BleedInterval, data.BleedDuration);
            }
        }

        public void RecieveMedi(iResourcePackReceiver receiver, float _)
        {
            var player = receiver.TryCast<PlayerAgent>();
            if (player != null && player.IsLocallyOwned)
            {
                _bleedingHandler.StopBleed();
            }
        }

        public void OnBuildStart()
        {
            var localPlayer = PlayerManager.GetLocalPlayerAgent();

            _bleedingHandler = localPlayer.gameObject.GetComponent<BleedingHandler>();
            if (_bleedingHandler == null)
            {
                _bleedingHandler = localPlayer.gameObject.AddComponent<BleedingHandler>();
            }
            _bleedingHandler.Agent = localPlayer;
        }

        public void OnLevelCleanup()
        {
            _bleedingHandler.StopBleed();
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