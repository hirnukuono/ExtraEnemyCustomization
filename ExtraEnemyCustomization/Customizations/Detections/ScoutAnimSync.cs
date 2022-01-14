using EECustom.Networking;
using EECustom.Utils;
using Enemies;
using SNetwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Detections
{
    public sealed class ScoutAnimSync : SyncedEvent<ScoutAnimPacket>
    {
        private static readonly Random _rand = new();

        public void DoRandom(EnemyAgent agent)
        {
            if (!SNet.IsMaster)
                return;

            var data = EnemyProperty<ScoutAnimOverrideData>.Get(agent);
            if (data == null)
                return;

            ScoutAnimType nextAnim = ScoutAnimType.Standing;
            if (data.ChanceToBend >= 1.0f)
            {
                nextAnim = ScoutAnimType.Bending;
            }
            else if (data.ChanceToBend <= 0.0f)
            {
                nextAnim = ScoutAnimType.Standing;
            }
            else
            {
                if (_rand.NextDouble() <= data.ChanceToBend)
                    nextAnim = ScoutAnimType.Bending;
            }

            var packet = new ScoutAnimPacket()
            {
                enemyID = agent.GlobalID,
                nextAnim = nextAnim
            };

            Send(packet);
        }

        public override void Receive(ScoutAnimPacket packet)
        {
            var data = EnemyProperty<ScoutAnimOverrideData>.Get(packet.enemyID);
            if (data == null)
                return;

            if (data.Agent.WasCollected)
                return;

            if (data.BendingWasCalled)
            {
                data.DoAnim(packet.nextAnim);

                data.NextAnim = ScoutAnimType.Unknown;
                data.BendingWasCalled = false;
                data.AnimDetermined = false;
            }
            else
            {
                data.NextAnim = packet.nextAnim;
                data.AnimDetermined = true;
            }
        }
    }

    public struct ScoutAnimPacket
    {
        public ushort enemyID;
        public ScoutAnimType nextAnim;
    }

    public class ScoutAnimOverrideData
    {
        public EnemyAgent Agent;
        public float ChanceToBend = 0.0f;
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

    public enum ScoutAnimType : byte
    {
        Unknown,
        Bending,
        Standing
    }
}
