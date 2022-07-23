using EEC.Attributes;
using Enemies;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class LimbDataModifier : MonoBehaviour
    {
        public Il2CppReferenceField<EnemyAgent> Agent;
        public Il2CppReferenceField<Dam_EnemyDamageLimb> Limb;
        public Il2CppValueField<eLimbDamageType> NewLimbType;
        public Il2CppValueField<float> NewHealth;
        public Il2CppValueField<float> NewMaxHealth;
        public Il2CppValueField<float> NewArmorMulti;
        public Il2CppValueField<float> NewWeakspotMulti;

        public void SetMulti(eLimbDamageType type, float armor, float weakspot)
        {
            NewLimbType.Set(type);
            NewArmorMulti.Set(armor);
            NewWeakspotMulti.Set(weakspot);
        }

        void Update()
        {
            if (Agent.Value.IsSetup)
            {
                var limb = Limb.Value;
                limb.m_type = NewLimbType.Value;
                limb.m_health = NewHealth.Value;
                limb.m_healthMax = NewMaxHealth.Value;
                limb.m_armorDamageMulti = NewArmorMulti.Value;
                limb.m_weakspotDamageMulti = NewWeakspotMulti.Value;
                enabled = false;
            }
        }
    }
}
