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
    public sealed class SpawnProjectileSetting
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

            var direction = BackwardDirection ? -fireAlign.forward : fireAlign.forward;
            return InLevelCoroutine.Start(SpawnProjectiles(new SpawnProjectileData()
            {
                Target = target,
                Align = fireAlign,
                KeepTrackAlign = keepTrack,
                BaseDirection = direction,
                OwnerID = owner.GlobalID,
                UpVector = fireAlign.up,
                RightVector = fireAlign.right
            }));
        }

        private IEnumerator SpawnProjectiles(SpawnProjectileData data)
        {
            if (data.Align == null)
                yield break;

            if (data.KeepTrackAlign)
            {
                data.LastSavedPosition = data.Align.position;
            }

            bool shouldWaitDelay = Delay > 0.0f;
            bool burstMode = BurstCount > 1;
            for (int i = 0; i < Count; i++)
            {
                if (burstMode)
                {
                    bool shouldWaitBurstDelay = BurstDelay > 0.0f;
                    for (int j = 1; j <= BurstCount; j++)
                    {
                        DoSpawn(data);
                        if (shouldWaitBurstDelay && j < BurstCount)
                        {
                            yield return WaitFor.Seconds[BurstDelay];
                        }
                    }
                }
                else
                {
                    DoSpawn(data);
                }

                if (shouldWaitDelay)
                {
                    yield return WaitFor.Seconds[Delay];
                }
            }
        }

        private void DoSpawn(SpawnProjectileData data)
        {
            var yaw = Rand.Range(ShotSpreadXMin, ShotSpreadXMax);
            var pitch = Rand.Range(ShotSpreadYMin, ShotSpreadYMax);
            var dirRot = Quaternion.LookRotation(data.BaseDirection);
            dirRot *= Quaternion.AngleAxis(pitch, data.UpVector);
            dirRot *= Quaternion.AngleAxis(yaw, data.RightVector);
            var newDir = dirRot * Vector3.forward;
            ProjectileManager.WantToFireTargeting(ProjectileType, data.Target, data.Position, newDir, data.OwnerID, 0f);
        }
    }

    public struct SpawnProjectileData
    {
        public Agent Target;
        public Transform Align;
        public Vector3 LastSavedPosition;
        public bool KeepTrackAlign;
        public Vector3 BaseDirection;
        public ushort OwnerID;
        public Vector3 UpVector;
        public Vector3 RightVector;

        public Vector3 Position
        {
            get
            {
                if (KeepTrackAlign && Align != null)
                {
                    return Align.position;
                }
                else
                {
                    return LastSavedPosition;
                }
            }
        }
    }
}