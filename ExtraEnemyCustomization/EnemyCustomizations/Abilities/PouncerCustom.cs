using EEC.Utils.Json.Elements;
using Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EEC.EnemyCustomizations.Abilities
{
    using Il2Cpp = PouncerDataContainer.Blocks;

    public sealed class PouncerCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public ValueBase DashCooldownMin { get; set; } = ValueBase.Unchanged;
        public ValueBase DashCooldownMax { get; set; } = ValueBase.Unchanged;
        public ValueBase DashMaxTime { get; set; } = ValueBase.Unchanged;
        public ValueBase ChargeDuration { get; set; } = ValueBase.Unchanged;
        public ValueBase ConsumeDuration { get; set; } = ValueBase.Unchanged;
        public ValueBase DashEndPhaseDistance { get; set; } = ValueBase.Unchanged;
        public ValueBase DashEndAnimationLength { get; set; } = ValueBase.Unchanged;
        public ValueBase ChargeStaggerDamageThreshold { get; set; } = ValueBase.Unchanged;
        public ValueBase DashStaggerDamageThreshold { get; set; } = ValueBase.Unchanged;
        public ValueBase StaggerDuration { get; set; } = ValueBase.Unchanged;
        public ValueBase FaceToTargetDuringProwlingDistance { get; set; } = ValueBase.Unchanged;
        public BoolBase StaggerLeadsToAfterHeld { get; set; } = BoolBase.Unchanged;
        public BoolBase EnableOffMeshLinkDash { get; set; } = BoolBase.Unchanged;
        public BoolBase LOSRequiredForDash { get; set; } = BoolBase.Unchanged;
        public BoolBase DashStaggerUsesHeavyHitreaction { get; set; } = BoolBase.Unchanged;
        public BoolBase Enable1PConsumeVFX { get; set; } = BoolBase.Unchanged;
        public BoolBase Enable3PConsumeVFX { get; set; } = BoolBase.Unchanged;
        public MovementModificationData DashMovementModifier { get; set; } = new();
        public HeldStateData HeldStateData { get; set; } = new();
        public DamageShapeData DamageShapeData { get; set; } = new();
        public PouncerSoundData PouncerSoundData { get; set; } = new();
        public HeldPathingData CombatStatePathingData { get; set; } = new();

        public override string GetProcessName()
        {
            return "Pouncer";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            var behaviour = agent.GetComponent<PouncerBehaviour>();
            var data = UnityEngine.Object.Instantiate(behaviour.Data);

            var cooldown = new Vector2
            {
                x = DashCooldownMin.GetAbsValue(data.m_DashCooldown.x),
                y = DashCooldownMax.GetAbsValue(data.m_DashCooldown.y)
            };
            data.m_DashCooldown = cooldown;
            data.DashMaxTime = DashMaxTime.GetAbsValue(data.DashMaxTime);
            data.ChargeDuration = ChargeDuration.GetAbsValue(data.ChargeDuration);
            data.ConsumeDuration = ConsumeDuration.GetAbsValue(data.ConsumeDuration);
            data.DashEndPhaseDistance = DashEndPhaseDistance.GetAbsValue(data.DashEndPhaseDistance);
            data.DashEndAnimationLength = DashEndAnimationLength.GetAbsValue(data.DashEndAnimationLength);
            data.ChargeStaggerDamageThreshold = ChargeStaggerDamageThreshold.GetAbsValue(data.ChargeStaggerDamageThreshold);
            data.DashStaggerDamageThreshold = DashStaggerDamageThreshold.GetAbsValue(data.DashStaggerDamageThreshold);
            data.DashStaggerDamageThreshold = DashStaggerDamageThreshold.GetAbsValue(data.DashStaggerDamageThreshold);
            data.StaggerDuration = StaggerDuration.GetAbsValue(data.StaggerDuration);
            data.FaceToTargetDuringProwlingDistance = FaceToTargetDuringProwlingDistance.GetAbsValue(data.FaceToTargetDuringProwlingDistance);

            //Bool
            data.StaggerLeadsToAfterHeld = StaggerLeadsToAfterHeld.GetValue(data.StaggerLeadsToAfterHeld);
            data.EnableOffMeshLinkDash = EnableOffMeshLinkDash.GetValue(data.EnableOffMeshLinkDash);
            data.LOSRequiredForDash = LOSRequiredForDash.GetValue(data.LOSRequiredForDash);
            data.DashStaggerUsesHeavyHitreaction = DashStaggerUsesHeavyHitreaction.GetValue(data.DashStaggerUsesHeavyHitreaction);
            data.Enable1PConsumeVFX = Enable1PConsumeVFX.GetValue(data.Enable1PConsumeVFX);
            data.Enable3PConsumeVFX = Enable3PConsumeVFX.GetValue(data.Enable3PConsumeVFX);

            //Data
            data.DashMovementModifier = DashMovementModifier.ToData(data.DashMovementModifier);
            data.HeldStateData = HeldStateData.ToData(data.HeldStateData);
            data.DamageShapeData = DamageShapeData.ToData(data.DamageShapeData);
            data.PouncerSoundData = PouncerSoundData.ToData();
            data.CombatStatePathingData = CombatStatePathingData.ToData(data.CombatStatePathingData);

            behaviour.m_data = data;
        }
    }

    public sealed class MovementModificationData
    {
        public ValueBase SpeedModifier { get; set; } = ValueBase.Unchanged;
        public ValueBase AccelerationModifier { get; set; } = ValueBase.Unchanged;

        public Il2Cpp.MovementModificationData ToData(Il2Cpp.MovementModificationData originalData)
        {
            return new()
            {
                SpeedModifier = SpeedModifier.GetAbsValue(originalData.SpeedModifier),
                AccelerationModifier = AccelerationModifier.GetAbsValue(originalData.AccelerationModifier)
            };
        }
    }

    public sealed class HeldStateData
    {
        public ValueBase HeldStartAnimationDuration { get; set; } = ValueBase.Unchanged;
        public ValueBase MaxHeldDuration { get; set; } = ValueBase.Unchanged;
        public ValueBase DamageToPlayerPerSecond { get; set; } = ValueBase.Unchanged;
        public ValueBase DamageOnStartHolding { get; set; } = ValueBase.Unchanged;
        public ValueBase AfterHeldRunAwayDuration { get; set; } = ValueBase.Unchanged;
        public ValueBase SpitOutStateDuration { get; set; } = ValueBase.Unchanged;
        public BoolBase ValidatePlayerDimension { get; set; } = BoolBase.Unchanged;
        public HeldPathingData PathingData { get; set; } = new();

        public Il2Cpp.HeldStateData ToData(Il2Cpp.HeldStateData orig)
        {
            return new()
            {
                HeldStartAnimationDuration = HeldStartAnimationDuration.GetAbsValue(orig.HeldStartAnimationDuration),
                MaxHeldDuration = MaxHeldDuration.GetAbsValue(orig.MaxHeldDuration),
                DamageToPlayerPerSecond = DamageToPlayerPerSecond.GetAbsValue(orig.DamageToPlayerPerSecond),
                DamageOnStartHolding = DamageOnStartHolding.GetAbsValue(orig.DamageOnStartHolding),
                AfterHeldRunAwayDuration = AfterHeldRunAwayDuration.GetAbsValue(orig.AfterHeldRunAwayDuration),
                SpitOutStateDuration = SpitOutStateDuration.GetAbsValue(orig.SpitOutStateDuration),
                ValidatePlayerDimension = ValidatePlayerDimension.GetValue(orig.ValidatePlayerDimension),
                PathingData = PathingData.ToData(orig.PathingData)
            };
        }
    }

    public sealed class DamageShapeData
    {
        public ValueBase Radius { get; set; } = ValueBase.Unchanged;
        public ValueBase Angle { get; set; } = ValueBase.Unchanged;

        public Il2Cpp.DamageShapeData ToData(Il2Cpp.DamageShapeData orig)
        {
            return new()
            {
                Radius = Radius.GetAbsValue(orig.Radius),
                Angle = Angle.GetAbsValue(orig.Angle)
            };
        }
    }

    public sealed class PouncerSoundData
    {
        public uint AttackCharge { get; set; } = 2040824805u;
        public uint AttackHit { get; set; } = 1034385728u;
        public uint AttackMiss { get; set; } = 1149851817u;
        public uint DashStart { get; set; } = 3726964003u;
        public uint HeldIdle { get; set; } = 376939216u;
        public uint HeldSpitOut { get; set; } = 2870456237u;
        public uint IdleGrowl { get; set; } = 3799706438u;
        public uint TentacleLoop { get; set; } = 3217748688u;

        public Il2Cpp.PouncerSoundData ToData()
        {
            return new()
            {
                AttackCharge = AttackCharge,
                AttackHit = AttackHit,
                AttackMiss = AttackMiss,
                DashStart = DashStart,
                HeldIdle = HeldIdle,
                HeldSpitOut = HeldSpitOut,
                IdleGrowl = IdleGrowl,
                TentacleLoop = TentacleLoop
            };
        }
    }

    public sealed class HeldPathingData
    {
        public ValueBase DstChangeRate { get; set; } = ValueBase.Unchanged;
        public ValueBase TryToKeepDistance { get; set; } = ValueBase.Unchanged;
        public BoolBase RecursiveReachableNodeSearch { get; set; } = BoolBase.Unchanged;
        public MovementModificationData MovementModifier { get; set; } = new();

        public Il2Cpp.HeldPathingData ToData(Il2Cpp.HeldPathingData orig)
        {
            return new()
            {
                DstChangeRate = DstChangeRate.GetAbsValue(orig.DstChangeRate),
                TryToKeepDistance = TryToKeepDistance.GetAbsValue(orig.TryToKeepDistance),
                RecursiveReachableNodeSearch = RecursiveReachableNodeSearch.GetValue(orig.RecursiveReachableNodeSearch),
                MovementModifier = MovementModifier.ToData(orig.MovementModifier)
            };
        }
    }
}
