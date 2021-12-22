using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    public interface IEmpTarget
    {
        uint ID { get; }
        Vector3 Position { get; }
        void Enable();
        void Disable();
    }
}
