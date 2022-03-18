using EEC.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models.Handlers
{
    [InjectToIl2Cpp]
    public sealed class BoneOffsetHandler : MonoBehaviour
    {
        public Animator Animator;
        public Vector3 Offset;
        public Vector3 RotationOffset;

        private void LateUpdate()
        {
            if (Animator.enabled)
            {
                transform.localPosition += Offset;
                transform.localEulerAngles += RotationOffset;
            }
        }
    }
}
