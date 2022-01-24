using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP
{
    public interface IEMPHandler
    {
        /// <summary>
        /// This method is called during the Update cycle when the EMP effect is active
        /// </summary>
        void Tick(bool enabled);

        /// <summary>
        /// This method is called when the EMP controller is assigned it's handler
        /// </summary>
        /// <param name="gameObject">The game object EMPController is attached to</param>
        void Setup(GameObject gameObject, EMPController controller);

        void ForceState(EMPState state);

        void OnDespawn();
    }
}
