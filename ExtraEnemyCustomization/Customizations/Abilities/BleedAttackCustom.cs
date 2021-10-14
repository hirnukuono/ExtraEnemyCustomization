using Agents;
using EECustom.Customizations.Abilities.Handlers;
using EECustom.Events;
using EECustom.Managers;
using EECustom.Utils;
using Enemies;
using Gear;
using Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EECustom.Customizations.Abilities
{
    public class BleedAttackCustom : EnemyCustomBase, IEnemySpawnedEvent, IEnemyDespawnedEvent
    {
        public BleedData MeleeData { get; set; } = new BleedData();
        public BleedData TentacleData { get; set; } = new BleedData();

        private readonly List<ushort> _EnemyList = new List<ushort>();
        private System.Random _Random = new System.Random();

        private BleedingHandler _BleedingHandler;

        public override string GetProcessName()
        {
            return "BleedAttack";
        }

        public override void OnConfigLoaded()
        {
            PlayerDamageEvents.OnMeleeDamage += OnMelee;
            PlayerDamageEvents.OnTentacleDamage += OnTentacle;
            LevelEvents.OnBuildStart += OnBuildStart;
            LevelEvents.OnLevelCleanup += OnLevelCleanup;

            if (ConfigManager.Current.AbilityCustom.CanMediStopBleeding)
                ResourcePackEvents.OnReceiveMedi += RecieveMedi;
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var id = agent.GlobalID;
            if (id == ushort.MaxValue)
                return;

            if (!_EnemyList.Contains(id))
            {
                _EnemyList.Add(id);
            }
        }

        public void OnDespawned(EnemyAgent agent)
        {
            _EnemyList.Remove(agent.GlobalID);
        }

        public void OnMelee(PlayerAgent player, Agent inflictor, float damage)
        {
            if (_EnemyList.Contains(inflictor.GlobalID))
            {
                var enemyAgent = inflictor.TryCast<EnemyAgent>();
                if (enemyAgent != null)
                    DoBleed(MeleeData);
            }
        }

        public void OnTentacle(PlayerAgent player, Agent inflictor, float damage)
        {
            if (_EnemyList.Contains(inflictor.GlobalID))
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