using EECustom.Attributes;
using Enemies;
using UnityEngine;

namespace EECustom.Patches.Handlers
{
    [InjectToIl2Cpp]
    public class FlyerStuckHandler : MonoBehaviour
    {
        public EnemyAgent Agent;
        public float UpdateInterval = 2.0f;
        public int RetryCount = 4;

        private Vector3 _firstPosition;
        private float _timer;
        private int _tryCount = -1;

        internal void Update()
        {
            if (_timer > Clock.Time)
                return;

            _timer = Clock.Time + UpdateInterval;

            if (_tryCount == -1)
            {
                _firstPosition = Agent.Position;
                _tryCount = 0;
                return;
            }

            if (Vector3.Distance(_firstPosition, Agent.Position) < 0.1f)
            {
                _tryCount++;

                if (_tryCount >= RetryCount)
                {
                    Logger.Debug("Flyer was stuck in Place!");
                    Agent.Damage.BulletDamage(10000.0f, null, Vector3.zero, Vector3.zero, Vector3.zero, false, 0, 1.0f, 1.0f);
                }
            }
            else
            {
                enabled = false;
            }
        }
    }
}