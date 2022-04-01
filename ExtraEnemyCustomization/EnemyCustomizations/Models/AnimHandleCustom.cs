using EEC.Utils.Json.Elements;
using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using static Enemies.EnemyLocomotion;

namespace EEC.EnemyCustomizations.Models
{
    public class AnimHandleCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public ValueBase TentacleAttackWindUpLen { get; set; } = ValueBase.Unchanged;
        public ValueBase JumpMoveSpeed { get; set; } = ValueBase.Unchanged;
        public ValueBase JumpLandLen { get; set; } = ValueBase.Unchanged;
        public ValueBase JumpStartLen { get; set; } = ValueBase.Unchanged;
        public BoolBase AllowMelee180 { get; set; } = BoolBase.Unchanged;
        public BoolBase NeedsArmsToMove { get; set; } = BoolBase.Unchanged;
        public BoolBase TwitchHit { get; set; } = BoolBase.Unchanged;
        public ValueBase StrafeRunSpeed { get; set; } = ValueBase.Unchanged;
        public ValueBase StrafeWalkSpeed { get; set; } = ValueBase.Unchanged;
        public ValueBase InPlayerRoomSpeed { get; set; } = ValueBase.Unchanged;
        public ValueBase RunSpeed { get; set; } = ValueBase.Unchanged;
        public ValueBase WalkSpeed { get; set; } = ValueBase.Unchanged;
        public ValueBase ClimbSpeed { get; set; } = ValueBase.Unchanged;
        public MeleeAttackData MeleeAttackFwd { get; set; } = new();
        public MeleeAttackData MeleeAttackBwd { get; set; } = new();

        public override string GetProcessName()
        {
            return "AnimHandle";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var handle = agent.Locomotion.AnimHandle;
            handle.TentacleAttackWindUpLen = TentacleAttackWindUpLen.GetAbsValue(handle.TentacleAttackWindUpLen);
            handle.JumpMoveSpeed = JumpMoveSpeed.GetAbsValue(handle.JumpMoveSpeed);
            handle.JumpLandLen = JumpLandLen.GetAbsValue(handle.JumpLandLen);
            handle.JumpStartLen = JumpStartLen.GetAbsValue(handle.JumpStartLen);
            handle.AllowMelee180 = AllowMelee180.GetValue(handle.AllowMelee180);
            handle.NeedsArmsToMove = NeedsArmsToMove.GetValue(handle.NeedsArmsToMove);
            handle.TwitchHit = TwitchHit.GetValue(handle.TwitchHit);
            handle.StrafeRunSpeed = StrafeRunSpeed.GetAbsValue(handle.StrafeRunSpeed);
            handle.StrafeWalkSpeed = StrafeWalkSpeed.GetAbsValue(handle.StrafeWalkSpeed);
            handle.InPlayerRoomSpeed = InPlayerRoomSpeed.GetAbsValue(handle.InPlayerRoomSpeed);
            handle.RunSpeed = RunSpeed.GetAbsValue(handle.RunSpeed);
            handle.WalkSpeed = WalkSpeed.GetAbsValue(handle.WalkSpeed);
            handle.ClimbSpeed = ClimbSpeed.GetAbsValue(handle.ClimbSpeed);
            handle.MeleeAttackFwd = MeleeAttackFwd.GetData(handle.MeleeAttackFwd);
            handle.MeleeAttackBwd = MeleeAttackBwd.GetData(handle.MeleeAttackBwd);

            var setting = agent.RegisterOrGetProperty<TentacleAttackSpeedProperty>();
            setting.TentacleAttackSpeedAdjustmentMult = agent.Locomotion.AnimHandle.TentacleAttackWindUpLen / handle.TentacleAttackWindUpLen;

            agent.Locomotion.AnimHandle = handle;
        }
    }

    public sealed class MeleeAttackData
    {
        public ValueBase Duration { get; set; } = ValueBase.Unchanged;
        public ValueBase DamageStart { get; set; } = ValueBase.Unchanged;
        public ValueBase DamageEnd { get; set; } = ValueBase.Unchanged;
        public bool ChangeSFX { get; set; } = false;
        public uint SFXId { get; set; }

        public MeleeAttackTimingData GetData(MeleeAttackTimingData original)
        {
            return new MeleeAttackTimingData()
            {
                Duration = Duration.GetAbsValue(original.Duration),
                DamageStart = DamageStart.GetAbsValue(original.DamageStart),
                DamageEnd = DamageEnd.GetAbsValue(original.DamageEnd),
                SFXId = ChangeSFX ? SFXId : original.SFXId
            };
        }
    }

    internal sealed class TentacleAttackSpeedProperty
    {
        public float TentacleAttackSpeedAdjustmentMult;
    }
}
