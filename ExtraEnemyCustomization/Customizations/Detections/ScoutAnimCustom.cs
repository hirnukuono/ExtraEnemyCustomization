using EECustom.Networking.Events;
using EECustom.Utils;
using Enemies;
using GTFO.API;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Detections
{
    public sealed class ScoutAnimCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        internal static readonly ScoutAnimSync _animSync = new();
        private static readonly Random _rand = new();

        public AnimationRandomType RandomType { get; set; } = AnimationRandomType.PerDetection;
        public EnemyAnimType BendAnimation { get; set; } = EnemyAnimType.AbilityUseOut;
        public EnemyAnimType StandAnimation { get; set; } = EnemyAnimType.AbilityUse;
        public float ChanceToBend { get; set; } = 1.0f;

        public bool OverridePullingAnimation { get; set; } = false;
        public EnemyAnimType PullingAnimation { get; set; } = EnemyAnimType.AbilityUseOut;

        static ScoutAnimCustom()
        {
            _animSync.Setup();
        }

        public override string GetProcessName()
        {
            return "ScoutAnim";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var data = EnemyProperty<ScoutAnimOverrideData>.RegisterOrGet(agent);
            data.Agent = agent;
            switch (RandomType)
            {
                case AnimationRandomType.PerEnemy:
                    data.ChanceToBend = ((float)_rand.NextDouble() <= ChanceToBend) ? 1.0f : 0.0f;
                    break;

                case AnimationRandomType.PerDetection:
                    data.ChanceToBend = ChanceToBend;
                    break;
            }
            
            data.BendAnimation = BendAnimation;
            data.StandAnimation = StandAnimation;
            data.OverridePullingAnimation = OverridePullingAnimation;
            data.PullingAnimation = PullingAnimation;
        }

        public enum AnimationRandomType
        {
            PerEnemy,
            PerDetection
        }
    }
}
