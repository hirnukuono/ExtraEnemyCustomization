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

        public EnemyAnimType BendAnimation { get; set; } = EnemyAnimType.AbilityUseOut;
        public EnemyAnimType StandAnimation { get; set; } = EnemyAnimType.AbilityUse;
        public float ChanceToBend { get; set; } = 1.0f;

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
            data.ChanceToBend = ChanceToBend;
            data.BendAnimation = BendAnimation;
            data.StandAnimation = StandAnimation;
        }
    }
}
