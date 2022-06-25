using EEC.Attributes;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models.Handlers
{
    [InjectToIl2Cpp]
    public sealed class BoneOffsetHandler : MonoBehaviour
    {
        public Il2CppReferenceField<Animator> Animator;
        public Il2CppValueField<Vector3> Offset;
        public Il2CppValueField<Vector3> RotationOffset;

        private void LateUpdate()
        {
            if (Animator.Get().enabled)
            {
                transform.localPosition += Offset.Get();
                transform.localEulerAngles += RotationOffset.Get();
            }
        }
    }
}
