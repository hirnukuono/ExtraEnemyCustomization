﻿using EEC.Networking;
using EEC.Networking.Events;
using Enemies;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EEC.Utils
{
    public static class EnemyAnimUtil
    {
        private static readonly Dictionary<EnemyAnimType, int[]> _animHashsLookup;
        private static bool _initialized = false;

        static EnemyAnimUtil()
        {
            _animHashsLookup = new(Enum.GetValues(typeof(EnemyAnimType)).Length);
        }

        internal static void Initialize()
        {
            if (_initialized)
                return;

            CacheLookup();

            _initialized = true;
        }

        private static void CacheLookup()
        {
            var enemyLocomotionType = typeof(EnemyLocomotion);

            foreach (EnemyAnimType type in Enum.GetValues(typeof(EnemyAnimType)))
            {
                if (type == EnemyAnimType.None)
                {
                    continue;
                }
                else if (type == EnemyAnimType.Heartbeats)
                {
                    _animHashsLookup.Add(type, EnemyLocomotion.s_hashHearbeats);
                    Logger.Verbose($"{type},  {string.Join(" / ", EnemyLocomotion.s_hashHearbeats)}");
                    continue;
                }

                var propName = $"s_hash{Enum.GetName(typeof(EnemyAnimType), type)}";
                var prop = enemyLocomotionType.GetProperty(propName, BindingFlags.Public | BindingFlags.Static);

                if (prop == null)
                {
                    Logger.Warning($"{propName} does not exist!");
                    continue;
                }

                if (prop.PropertyType == typeof(int))
                {
                    _animHashsLookup.Add(type, new int[] { (int)prop.GetValue(null) });
                    Logger.Verbose($"{type},  {prop.GetValue(null)}");
                }
                else if (prop.PropertyType == typeof(Il2CppStructArray<int>))
                {
                    int[] values = (Il2CppStructArray<int>)prop.GetValue(null);
                    _animHashsLookup.Add(type, values);
                    Logger.Verbose($"{type},  {string.Join(" / ", values)}");
                }
                else
                {
                    Logger.Error($"{type} is not a valid hash property!");
                }
            }
        }

        public static void DoAnimationLocal(EnemyAgent agent, EnemyAnimType type, float crossfadeTime, bool pauseAI)
        {
            if (!_initialized)
            {
                Logger.Error("EnemyAnimUtil.DoAnimation was called too fast before it got cached!");
                return;
            }

            if (type == EnemyAnimType.None)
            {
                return;
            }

            if (!_animHashsLookup.TryGetValue(type, out var hashes))
            {
                Logger.Error($"Cannot find AnimationHash with: {type}");
                return;
            }

            var index = hashes.Length > 1 ? Rand.IndexOf(hashes) : 0;
            agent.Locomotion.m_animator.applyRootMotion = true;
            agent.Locomotion.m_animator.CrossFadeInFixedTime(hashes[index], crossfadeTime);

            if (pauseAI)
            {
                agent.AI.m_navMeshAgent.velocity = UnityEngine.Vector3.zero;
                if (agent.AI.m_navMeshAgent.isOnNavMesh)
                {
                    agent.AI.m_navMeshAgent.isStopped = true;
                }
            }
        }

        public static void DoAnimation(EnemyAgent agent, EnemyAnimType type, float crossfadeTime, bool pauseAI)
        {
            if (!_initialized)
            {
                Logger.Error("EnemyAnimUtil.DoAnimation was called too fast before it got cached!");
                return;
            }

            if (type == EnemyAnimType.None)
            {
                return;
            }

            if (!_animHashsLookup.TryGetValue(type, out var hashes))
            {
                Logger.Error($"Cannot find AnimationHash with: {type}");
                return;
            }

            var index = hashes.Length > 1 ? Rand.IndexOf(hashes) : 0;
            NetworkManager.EnemyAnim.Send(new EnemyAnimEvent.Packet()
            {
                enemyID = agent.GlobalID,
                crossfadeTime = crossfadeTime,
                pauseAI = pauseAI,
                animHash = hashes[index]
            });
        }
    }

    public enum EnemyAnimType : byte
    {
        None,
        MoveOnGround,
        Forward,
        Right,
        ClimbLadder,
        GiveBirth,
        HitLights_Fwd,
        HitLights_Bwd,
        HitLights_Rt,
        HitLights_Lt,
        HitHeavys_Fwd,
        HitHeavys_Bwd,
        HitHeavys_Rt,
        HitHeavys_Lt,
        Screams,
        ScreamTurns,
        HibernationIn,
        Heartbeats,
        HibernationWakeups,
        HibernationWakeupTurns,
        AbilityFires,
        AbilityUse,
        AbilityUseOut,
        MeleeWalkSequences,
        MeleeSequences,
        Melee180Sequences,
        JumpStart,
        JumpLand
    }
}