using EECustom.Utils;
using Enemies;
using GTFO.API;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Detections
{
    public class ScoutAnimCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public const string EventName = "EEC_ScoutAnim_Sync";

        private static readonly Random _rand = new();

        public EnemyAnimType BendAnimation { get; set; } = EnemyAnimType.AbilityUseOut;
        public EnemyAnimType StandAnimation { get; set; } = EnemyAnimType.AbilityUse;
        public float ChanceForBend { get; set; } = 1.0f;

        static ScoutAnimCustom()
        {
            NetworkAPI.RegisterEvent<ScoutAnimPacket>(EventName, ReceivedAnimPacket);
        }

        public static void DoAnimRandom(EnemyAgent agent)
        {
            if (!SNet.IsMaster)
                return;

            var data = EnemyProperty<ScoutAnimOverrideData>.Get(agent);
            if (data == null)
                return;

            ScoutAnimType nextAnim = ScoutAnimType.Standing;
            if (data.ChanceForBend >= 1.0f)
            {
                nextAnim = ScoutAnimType.Bending;
            }
            else if (data.ChanceForBend <= 0.0f)
            {
                nextAnim = ScoutAnimType.Standing;
            }
            else
            {
                if (_rand.NextDouble() <= data.ChanceForBend)
                    nextAnim = ScoutAnimType.Bending;
            }

            var packet = new ScoutAnimPacket()
            {
                enemyID = agent.GlobalID,
                nextAnim = nextAnim
            };

            NetworkAPI.InvokeEvent(EventName, packet);
            ReceivedAnimPacket(0, packet);
        }

        public override string GetProcessName()
        {
            return "ScoutAnim";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var data = EnemyProperty<ScoutAnimOverrideData>.RegisterOrGet(agent);
            data.Agent = agent;
            data.ChanceForBend = ChanceForBend;
            data.BendAnimation = BendAnimation;
            data.StandAnimation = StandAnimation;
        }

        private static void ReceivedAnimPacket(ulong id, ScoutAnimPacket anim)
        {
            var data = EnemyProperty<ScoutAnimOverrideData>.Get(anim.enemyID);
            if (data == null)
                return;

            if (data.Agent.WasCollected)
                return;

            if (data.BendingWasCalled)
            {
                data.DoAnim(anim.nextAnim);

                data.NextAnim = ScoutAnimType.Unknown;
                data.BendingWasCalled = false;
                data.AnimDetermined = false;
            }
            else
            {
                data.NextAnim = anim.nextAnim;
                data.AnimDetermined = true;
            }
        }
    }

    public class ScoutAnimOverrideData
    {
        public EnemyAgent Agent;
        public float ChanceForBend = 0.0f;
        public bool AnimDetermined = false;
        public bool BendingWasCalled = false;
        public ScoutAnimType NextAnim = ScoutAnimType.Unknown;
        public EnemyAnimType BendAnimation = EnemyAnimType.AbilityUseOut;
        public EnemyAnimType StandAnimation = EnemyAnimType.AbilityUse;

        public void DoAnim(ScoutAnimType anim)
        {
            if (anim == ScoutAnimType.Standing)
                EnemyAnimUtil.DoAnimationLocal(Agent, StandAnimation, 0.15f, false);
            else if (anim == ScoutAnimType.Bending)
                EnemyAnimUtil.DoAnimationLocal(Agent, BendAnimation, 0.15f, false);
        }
    }

    public struct ScoutAnimPacket
    {
        public ushort enemyID;
        public ScoutAnimType nextAnim;
    }

    public enum ScoutAnimType : byte
    {
        Unknown,
        Bending,
        Standing
    }
}
