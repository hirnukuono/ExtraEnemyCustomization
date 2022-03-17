using EEC.Events;
using Enemies;
using GTFO.API;
using System;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models
{
    public sealed class BoneCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public BoneTransform[] Bones { get; set; } = Array.Empty<BoneTransform>();
        public BonePrefab[] Prefabs { get; set; } = Array.Empty<BonePrefab>();

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

            foreach (var bonePrefab in Prefabs)
            {
                TryApplyBonePrefab(agent, bonePrefab);
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

                if (Logger.VerboseLogAllowed)
                    LogVerbose($"Applied Bone Setting: {transform.name}, offset: {boneTransform.Offset}, scale: {boneTransform.Scale} rotationOffset: {boneTransform.RotationOffset}");
            }
            catch (Exception e)
            {
                LogError($"Bone Transform owner: [{agent.name}] bone: [{boneTransform.Bone}] were not set! : {e}");
            }
        }

        private void TryApplyBonePrefab(EnemyAgent agent, BonePrefab bonePrefab)
        {
            try
            {
                var transform = agent.Anim.GetBoneTransform(bonePrefab.Bone);
                if (transform == null)
                    throw new NullReferenceException("Bone is missing in prefab!");

                var prefab = AssetAPI.GetLoadedAsset(bonePrefab.Prefab);
                if (prefab == null)
                    throw new NullReferenceException($"Prefab '{bonePrefab.Prefab}' is missing!");

                var boneAttach = UnityEngine.Object.Instantiate(prefab, transform).Cast<GameObject>();
                boneAttach.transform.localPosition = bonePrefab.Position;
                boneAttach.transform.localEulerAngles = bonePrefab.Rotation;
                boneAttach.transform.localScale = bonePrefab.Scale;

                LogVerbose($"Attached Bone Prefab: {transform.name} '{bonePrefab.Prefab}', pos: {bonePrefab.Position}, scale: {bonePrefab.Scale} rot: {bonePrefab.Rotation}");
            }
            catch (Exception e)
            {
                LogError($"Bone Transform owner: [{agent.name}] bone: [{bonePrefab.Bone}] prefab: [{bonePrefab.Prefab}] were not attached! : {e}");
            }
        }

        public sealed class BoneTransform
        {
            public HumanBodyBones Bone { get; set; } = HumanBodyBones.Head;
            public bool UsingRelativeScale { get; set; } = false;
            public Vector3 Scale { get; set; } = Vector3.zero;
            public Vector3 Offset { get; set; } = Vector3.zero;
            public Vector3 RotationOffset { get; set; } = Vector3.zero;
        }

        public sealed class BonePrefab
        {
            public HumanBodyBones Bone { get; set; } = HumanBodyBones.Head;
            public string Prefab { get; set; } = string.Empty;
            public Vector3 Scale { get; set; } = Vector3.zero;
            public Vector3 Position { get; set; } = Vector3.zero;
            public Vector3 Rotation { get; set; } = Vector3.zero;
        }
    }
}