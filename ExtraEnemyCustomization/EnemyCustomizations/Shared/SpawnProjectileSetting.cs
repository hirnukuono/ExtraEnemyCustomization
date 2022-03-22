using Agents;
using EEC.Utils;
using EEC.Utils.Unity;
using Enemies;
using SNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EEC.EnemyCustomizations.Shared
{
    public interface ISpawnProjectileSetting
    {
        public ProjectileType ProjectileType { get; set; }
        public bool BackwardDirection { get; set; }
        public int Count { get; set; }
        public int BurstCount { get; set; }
        public float Delay { get; set; }
        public float BurstDelay { get; set; }
        public float ShotSpreadXMin { get; set; }
        public float ShotSpreadXMax { get; set; }
        public float ShotSpreadYMin { get; set; }
        public float ShotSpreadYMax { get; set; }
    }

    public sealed class SpawnProjectileSetting : ISpawnProjectileSetting
    {
        public bool Enabled { get; set; } = false;
        public ProjectileType ProjectileType { get; set; } = ProjectileType.TargetingSmall;
        public bool BackwardDirection { get; set; } = false;
        public int Count { get; set; } = 1;
        public int BurstCount { get; set; } = 1;
        public float Delay { get; set; } = 0.1f;
        public float BurstDelay { get; set; } = 0.05f;
        public float ShotSpreadXMin { get; set; } = 0.0f;
        public float ShotSpreadXMax { get; set; } = 0.0f;
        public float ShotSpreadYMin { get; set; } = 0.0f;
        public float ShotSpreadYMax { get; set; } = 0.0f;

        public Coroutine DoSpawn(EnemyAgent owner, Agent target, Transform fireAlign, bool keepTrack)
        {
            if (!Enabled)
                return null;

            if (!SNet.IsMaster)
                return null;

            return SpawnProjectileExtension.DoSpawn(this, owner, target, fireAlign, keepTrack);
        }
    }
}