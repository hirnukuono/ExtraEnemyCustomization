using Enemies;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public sealed class ModelRefCustom : RevertableEnemyCustomBase, IEnemyPrefabBuiltEvent
    {
        public ModelRefData[] ModelRefs { get; set; } = new ModelRefData[0];

        public override string GetProcessName()
        {
            return "MedelRef";
        }

        public void OnPrefabBuilt(EnemyAgent agent)
        {
            var modelRef = agent.ModelRef;
            var changeCache = new ModelRefCache();
            changeCache.CopyFrom(modelRef);

            var originalCache = changeCache;
            PushRevertJob(() =>
            {
                originalCache.CopyTo(ref modelRef);
            });

            foreach (var mRef in ModelRefs)
            {
                Transform baseTransform = null;
                switch (mRef.CopyFrom)
                {
                    case BaseModelRefType.Tentacle:
                        baseTransform = modelRef.m_tentacleAlign;
                        break;

                    case BaseModelRefType.Tentacle2:
                        baseTransform = modelRef.m_tentacleAlign2;
                        break;

                    case BaseModelRefType.Tentacle3:
                        baseTransform = modelRef.m_tentacleAlign3;
                        break;

                    case BaseModelRefType.TentacleNoHead:
                        baseTransform = modelRef.m_tentacleAlignNoHead;
                        break;

                    case BaseModelRefType.TentacleNoCheat:
                        baseTransform = modelRef.m_tentacleAlignNoChest;
                        break;

                    case BaseModelRefType.ShooterFire:
                        baseTransform = modelRef.m_shooterFireAlign;
                        break;

                    case BaseModelRefType.ScoutFeeler:
                        baseTransform = modelRef.m_detectAbilityAlign;
                        break;

                    case BaseModelRefType.BioMarker:
                        baseTransform = modelRef.m_markerTagAlign.transform;
                        break;

                    default:
                        if (!string.IsNullOrEmpty(mRef.CreateFromPath))
                            baseTransform = agent.transform.FindChild(mRef.CreateFromPath);
                        break;
                }

                if (baseTransform == null)
                {
                    LogError($"Could not find base modelRef to work on, type: {mRef.Type} path: {mRef.CreateFromPath}");
                    continue;
                }

                var newObj = new GameObject("New_ModelRef");
                newObj.transform.parent = baseTransform.parent;
                newObj.transform.position = baseTransform.position;
                newObj.transform.rotation = baseTransform.rotation;

                newObj.transform.localPosition = baseTransform.localPosition + mRef.Offset;
                newObj.transform.localRotation = baseTransform.localRotation;

                var newAngle = newObj.transform.localRotation.eulerAngles + mRef.RotateOffset;
                newObj.transform.localRotation.SetEulerAngles(newAngle);

                switch (mRef.Type)
                {
                    case BaseModelRefType.Tentacle:
                        changeCache.m_tentacleAlign = newObj.transform;
                        break;

                    case BaseModelRefType.Tentacle2:
                        changeCache.m_tentacleAlign2 = newObj.transform;
                        break;

                    case BaseModelRefType.Tentacle3:
                        changeCache.m_tentacleAlign3 = newObj.transform;
                        break;

                    case BaseModelRefType.TentacleNoHead:
                        changeCache.m_tentacleAlignNoHead = newObj.transform;
                        break;

                    case BaseModelRefType.TentacleNoCheat:
                        changeCache.m_tentacleAlignNoChest = newObj.transform;
                        break;

                    case BaseModelRefType.ShooterFire:
                        changeCache.m_shooterFireAlign = newObj.transform;
                        break;

                    case BaseModelRefType.ScoutFeeler:
                        changeCache.m_detectAbilityAlign = newObj.transform;
                        break;

                    case BaseModelRefType.BioMarker:
                        changeCache.m_markerTagAlign = newObj.transform;
                        break;

                    default:
                        LogError($"Type is not valid: {mRef.Type}");
                        continue;
                }

                LogDev("Done!");
            }
            changeCache.CopyTo(ref modelRef);
        }
    }

    public class ModelRefData
    {
        public BaseModelRefType Type { get; set; } = BaseModelRefType.None;
        public BaseModelRefType CopyFrom { get; set; } = BaseModelRefType.None;
        public string CreateFromPath { get; set; } = string.Empty;
        public Vector3 Offset { get; set; } = Vector3.zero;
        public Vector3 RotateOffset { get; set; } = Vector3.zero;
    }

    public enum BaseModelRefType
    {
        None,
        Tentacle,
        Tentacle2,
        Tentacle3,
        TentacleNoHead,
        TentacleNoCheat,
        ShooterFire,
        ScoutFeeler,
        BioMarker
    }

    public struct ModelRefCache
    {
        public Transform m_tentacleAlign;
        public Transform m_tentacleAlign2;
        public Transform m_tentacleAlign3;
        public Transform m_tentacleAlignNoHead;
        public Transform m_tentacleAlignNoChest;
        public Transform m_shooterFireAlign;
        public Transform m_detectAbilityAlign;
        public Transform m_markerTagAlign;

        public void CopyFrom(EnemyModelRefs modelRef)
        {
            m_tentacleAlign = modelRef.m_tentacleAlign;
            m_tentacleAlign2 = modelRef.m_tentacleAlign2;
            m_tentacleAlign3 = modelRef.m_tentacleAlign3;
            m_tentacleAlignNoHead = modelRef.m_tentacleAlignNoHead;
            m_tentacleAlignNoChest = modelRef.m_tentacleAlignNoChest;
            m_shooterFireAlign = modelRef.m_shooterFireAlign;
            m_detectAbilityAlign = modelRef.m_detectAbilityAlign;
            m_markerTagAlign = modelRef.m_markerTagAlign.transform;
        }

        public void CopyTo(ref EnemyModelRefs modelRef)
        {
            modelRef.m_tentacleAlign = m_tentacleAlign;
            modelRef.m_tentacleAlign2 = m_tentacleAlign2;
            modelRef.m_tentacleAlign3 = m_tentacleAlign3;
            modelRef.m_tentacleAlignNoHead = m_tentacleAlignNoHead;
            modelRef.m_tentacleAlignNoChest = m_tentacleAlignNoChest;
            modelRef.m_shooterFireAlign = m_shooterFireAlign;
            modelRef.m_detectAbilityAlign = m_detectAbilityAlign;
            modelRef.m_markerTagAlign = m_markerTagAlign.gameObject;
        }
    }
}