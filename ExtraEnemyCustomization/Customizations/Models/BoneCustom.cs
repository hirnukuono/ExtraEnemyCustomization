using EECustom.Events;
using Enemies;
using System;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public sealed class BoneCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public BoneTransform[] Bones { get; set; } = new BoneTransform[0];

        public override string GetProcessName()
        {
            return "Bone";
        }

        public void OnSpawned(EnemyAgent agent)
        {
            foreach (var boneTransform in Bones)
            {
                TryApplyBoneTransform(agent, boneTransform);
            }
        }

        private void TryApplyBoneTransform(EnemyAgent agent, BoneTransform boneTransform)
        {
            try
            {
                var transform = agent.Anim.GetBoneTransform(boneTransform.Bone);
                if (boneTransform.Offset != Vector3.zero || boneTransform.Offset != Vector3.zero)
                {
                    MonoBehaviourEventHandler.AttatchToObject(transform.gameObject, onLateUpdate: (GameObject g) =>
                    {
                        if (agent.Locomotion.m_animator.enabled)
                        {
                            transform.localPosition += boneTransform.Offset;
                            transform.localEulerAngles += boneTransform.RotationOffset;
                        }
                    });
                }
                
                if (boneTransform.UsingRelativeScale)
                {
                    var newScale = new Vector3()
                    {
                        x = transform.localScale.x * boneTransform.Scale.x,
                        y = transform.localScale.y * boneTransform.Scale.y,
                        z = transform.localScale.z * boneTransform.Scale.z
                    };
                    transform.localScale = newScale;
                }
                else
                {
                    transform.localScale = boneTransform.Scale;
                }
                
                LogVerbose($"Applied Bone Setting: {transform.name}, offset: {boneTransform.Offset}, scale: {boneTransform.Scale} rotationOffset: {boneTransform.RotationOffset}");
            }
            catch (Exception e)
            {
                LogError($"Bone Transform owner: [{agent.name}] bone: [{boneTransform.Bone}] were not set! : {e}");
            }
        }
    }

    public class BoneTransform
    {
        public HumanBodyBones Bone { get; set; } = HumanBodyBones.Head;
        public bool UsingRelativeScale { get; set; } = false;
        public Vector3 Scale { get; set; } = Vector3.zero;
        public Vector3 Offset { get; set; } = Vector3.zero;
        public Vector3 RotationOffset { get; set; } = Vector3.zero;
    }
}