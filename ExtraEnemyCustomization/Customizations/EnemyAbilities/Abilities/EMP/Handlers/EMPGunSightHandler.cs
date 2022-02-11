using EECustom.Extensions;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers
{
    public class EMPGunSightHandler : EMPHandlerBase
    {
        public GameObject[] _sightPictures;

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            if (renderers != null)
            {
                _sightPictures = renderers
                    .Where(x => x.material?.shader?.name?.Contains("HolographicSight", StringComparison.InvariantCultureIgnoreCase) ?? false)
                    .Select(x => x.gameObject)
                    .ToArray();
            }

            if (_sightPictures == null || _sightPictures.Length < 1)
            {
                Logger.Warning("Unable to find sight on {0}!", gameObject.name);
                return;
            }
        }

        protected override void DeviceOff()
        {
            ForEachSights(x => x.SetActive(false));
        }

        protected override void DeviceOn()
        {
            ForEachSights(x => x.SetActive(true));
        }

        protected override void FlickerDevice()
        {
            ForEachSights(x => x.SetActive(FlickerUtil()));
        }

        private void ForEachSights(Action<GameObject> action)
        {
            if (_sightPictures == null)
                return;

            foreach (var sight in _sightPictures)
            {
                if (sight == null)
                    continue;

                action?.Invoke(sight);
            }
        }
    }
}