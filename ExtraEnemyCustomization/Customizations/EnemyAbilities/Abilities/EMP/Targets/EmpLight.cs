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
        public Vector3 Position => transform.position;
        public uint ID => _id;
        private uint _id;
        private LG_Light _light;
        private float _originalIntensity;

        public EmpLight(IntPtr ptr) : base(ptr)
        {
        }

        void Awake()
        {
            _light = GetComponent<LG_Light>();
            if (_light == null)
            {
                Logger.Error("No light found on game object EmpLight was added to!");
                Destroy(this);
                return;
            }
            _originalIntensity = _light.GetIntensity();
            _id = EMPManager.GetID();
            EMPManager.AddTarget(this);
        }

        void OnDestroy()
        {
            EMPManager.RemoveTarget(this);
        }

        public void EnableEmp()
        {
            if (_light != null)
                _light.ChangeIntensity(0);
        }

        public void DisableEmp()
        {
            if (_light != null)
                _light.ChangeIntensity(_originalIntensity);
        }
    }
}
