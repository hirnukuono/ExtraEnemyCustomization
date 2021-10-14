using Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Customizations.Abilities
{
    //MAJOR: Implement
    public class DoorBreakerCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public bool UseGlobalTimer { get; set; } = false;
        public float Damage { get; set; } = 1.0f;
        public float MinDelay { get; set; } = 0.5f;
        public float MaxDelay { get; set; } = 0.75f;

        private float _GlobalTimer = 0.0f;
        

        public override string GetProcessName()
        {
            return "DoorBreaker";
        }

        public override void OnConfigLoaded()
        {
            
        }

        public void OnSpawned(EnemyAgent agent)
        {
            //agent.AI.m_behaviour.m_currentStateName
            throw new NotImplementedException();
            //weakdoor.m_sync.AttemptDoorInteraction(eDoorInteractionType.DoDamage, 1.0f, 0f, sourcePos, null);
        }
    }
}
