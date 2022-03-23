using Agents;
using EEC.Utils;
using EEC.Utils.Unity;
using Enemies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EEC.EnemyCustomizations.Shared
{
    public static class SpawnProjectileExtension
    {
        public static Coroutine DoSpawn(this ISpawnProjectileSetting setting, EnemyAgent owner, Agent target, Transform fireAlign, bool keepTrack)
        {
            var direction = setting.BackwardDirection ? -fireAlign.forward : fireAlign.forward;
            return InLevelCoroutine.Start(SpawnProjectiles(setting, new SpawnProjectileData()
            {
                Target = target,
                Align = fireAlign,
                LastSavedPosition = fireAlign.position,
                KeepTrackAlign = keepTrack,
                BaseDirection = direction,
                OwnerID = owner.GlobalID,
                UpVector = fireAlign.up,
                RightVector = fireAlign.right
            }));
        }

        private static IEnumerator SpawnProjectiles(ISpawnProjectileSetting setting, SpawnProjectileData data)
        {
            if (data.Align == null)
                yield break;

            bool shouldWaitDelay = setting.Delay > 0.0f;
            bool burstMode = setting.BurstCount > 1;
            for (int i = 0; i < setting.Count; i++)
            {
                if (burstMode)
                {
                    bool shouldWaitBurstDelay = setting.BurstDelay > 0.0f;
                    for (int j = 1; j <= setting.BurstCount; j++)
                    {
                        Spawn(setting, data);
                        if (shouldWaitBurstDelay && j < setting.BurstCount)
                        {
                            yield return WaitFor.Seconds[setting.BurstDelay];
                        }
                    }
                }
                else
                {
                    Spawn(setting, data);
                }

                if (shouldWaitDelay)
                {
                    yield return WaitFor.Seconds[setting.Delay];
                }
            }
        }

        private static void Spawn(ISpawnProjectileSetting setting, SpawnProjectileData data)
        {
            var yaw = Rand.Range(setting.ShotSpreadXMin, setting.ShotSpreadXMax);
            var pitch = Rand.Range(setting.ShotSpreadYMin, setting.ShotSpreadYMax);
            var dirRot = Quaternion.LookRotation(data.BaseDirection);
            dirRot *= Quaternion.AngleAxis(pitch, data.UpVector);
            dirRot *= Quaternion.AngleAxis(yaw, data.RightVector);
            var newDir = dirRot * Vector3.forward;
            ProjectileManager.WantToFireTargeting(setting.ProjectileType, data.Target, data.Position, newDir, data.OwnerID, 0f);
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
}
