using EECustom.Attributes;
using LevelGeneration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Targets
{
    [InjectToIl2Cpp]
    public class EmpLight : MonoBehaviour, IEmpTarget
    {
        private LG_Light light;
        private float originalIntensity;

        public EmpLight(IntPtr ptr) : base(ptr)
        {
        }

        void Awake()
        {
            light = GetComponent<LG_Light>();
            originalIntensity = light.GetIntensity();
        }

        public void Disable()
        {
            
        }

        public void Enable()
        {
            
        }
    }
}
