using EECustom.Networking;
using EECustom.Utils;
using Enemies;
using SNetwork;

namespace EECustom.Customizations.Detections
{
    public sealed class ScoutAnimSync : SyncedEvent<ScoutAnimSync.Packet>
    {
        public override string GUID => "SAM";

        public void DoRandom(EnemyAgent agent)
        {
            if (!SNet.IsMaster)
                return;

            if (!agent.TryGetProperty<ScoutAnimOverrideProperty>(out var data))
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
                nextAnim = Rand.CanDoBy(data.ChanceToBend) ?
                    ScoutAnimType.Bending :
                    ScoutAnimType.Standing;
            }

            var packet = new Packet()
            {
                enemyID = agent.GlobalID,
                nextAnim = nextAnim
            };

            Send(packet);
        }

        public override void Receive(Packet packet)
        {
            if (!EnemyProperty<ScoutAnimOverrideProperty>.TryGet(packet.enemyID, out var data))
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

        public struct Packet
        {
            public ushort enemyID;
            public ScoutAnimType nextAnim;
        }
    }

    internal sealed class ScoutAnimOverrideProperty
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