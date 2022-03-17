using System;
using System.Linq;
using UnityEngine;

namespace EEC.CustomAbilities.EMP.Handlers
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
                    .Where(x => x.sharedMaterial != null && x.sharedMaterial.shader != null)
                    .Where(x => x.sharedMaterial.shader.name.InvariantContains("HolographicSight"))
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