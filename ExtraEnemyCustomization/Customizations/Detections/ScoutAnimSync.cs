﻿using EECustom.Networking;
using EECustom.Utils;
using Enemies;
using SNetwork;
using System;

namespace EECustom.Customizations.Detections
{
    public sealed class ScoutAnimSync : SyncedEvent<ScoutAnimPacket>
    {
        private static readonly Random _rand = new();

        public void DoRandom(EnemyAgent agent)
        {
            if (!SNet.IsMaster)
                return;

            if (!EnemyProperty<ScoutAnimOverrideData>.TryGet(agent, out var data))
                return;

            ScoutAnimType nextAnim;
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
                nextAnim = (_rand.NextDouble() <= data.ChanceToBend) ?
                    ScoutAnimType.Bending :
                    ScoutAnimType.Standing;
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
            if (!EnemyProperty<ScoutAnimOverrideData>.TryGet(packet.enemyID, out var data))
                return;

            if (data.Agent.WasCollected)
                return;

            if (data.EnterWasCalled)
            {
                data.DoAnim(packet.nextAnim);
                data.AnimDetermined = false;
                data.EnterWasCalled = false;
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
        public bool EnterWasCalled = false;
        public ScoutAnimType NextAnim = ScoutAnimType.Unknown;
        public EnemyAnimType BendAnimation = EnemyAnimType.AbilityUseOut;
        public EnemyAnimType StandAnimation = EnemyAnimType.AbilityUse;

        public bool OverridePullingAnimation = false;
        public EnemyAnimType PullingAnimation = EnemyAnimType.AbilityUseOut;

        public void DoAnim(ScoutAnimType anim)
        {
            if (anim == ScoutAnimType.Standing)
                EnemyAnimUtil.DoAnimationLocal(Agent, StandAnimation, 0.15f, false);
            else if (anim == ScoutAnimType.Bending)
                EnemyAnimUtil.DoAnimationLocal(Agent, BendAnimation, 0.15f, false);
        }

        public void DoPullingAnim()
        {
            EnemyAnimUtil.DoAnimationLocal(Agent, PullingAnimation, 0.15f, false);
        }
    }

    public enum ScoutAnimType : byte
    {
        Unknown,
        Bending,
        Standing
    }
}