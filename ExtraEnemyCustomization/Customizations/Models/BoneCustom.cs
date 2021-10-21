using Enemies;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public class BoneCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public BoneTransform[] Transforms { get; set; } = new BoneTransform[0];

        public override string GetProcessName()
        {
            return "Bone";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            foreach (var boneTransform in Transforms)
            {
                TryApplyBoneTransform(agent, boneTransform);
            }
        }

        private void TryApplyBoneTransform(EnemyAgent agent, BoneTransform boneTransform)
        {
            try
            {
                var transform = agent.Anim.GetBoneTransform(boneTransform.Bone);
                transform.localPosition += boneTransform.Offset;
                transform.localScale = boneTransform.Scale;
                transform.localRotation.SetEulerAngles(transform.localEulerAngles + boneTransform.RotationOffset);
                LogVerbose($"Applied Bone Setting: {transform.name}, offset: {boneTransform.Offset.ToString()}, scale: {boneTransform.Scale.ToString()} rotationOffset: {boneTransform.RotationOffset.ToString()}");
            }
            catch(Exception e)
            {
                LogError($"Bone Transform were not set! : {e}");
            }
        }
    }

    public class BoneTransform
    {
        public HumanBodyBones Bone { get; set; } = HumanBodyBones.Head;
        public Vector3 Offset { get; set; } = Vector3.zero;
        public Vector3 Scale { get; set; } = Vector3.zero;
        public Vector3 RotationOffset { get; set; } = Vector3.zero;
    }
}
