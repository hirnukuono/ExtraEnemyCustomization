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

        public float ChanceToBending { get; set; } = 1.0f;

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
            if (data.ChanceToBending >= 1.0f)
            {
                nextAnim = ScoutAnimType.Bending;
            }
            else if (data.ChanceToBending <= 0.0f)
            {
                nextAnim = ScoutAnimType.Standing;
            }
            else
            {
                if (_rand.NextDouble() <= data.ChanceToBending)
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
            data.ChanceToBending = ChanceToBending;
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
                if (anim.nextAnim == ScoutAnimType.Standing)
                {
                    EnemyAnimUtil.DoAnimationLocal(data.Agent, EnemyAnimType.AbilityUse, 0.15f, false);
                }

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
        public float ChanceToBending = 0.0f;
        public bool AnimDetermined = false;
        public bool BendingWasCalled = false;
        public ScoutAnimType NextAnim = ScoutAnimType.Unknown;
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
