using EECustom.Extensions;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EECustom.Customizations.EnemyAbilities.Abilities.EMP.Handlers
{
    public class EMPGunSightHandler : EMPHandlerBase
    {
        public GameObject _sightPicture;
        private const string _regex = @"Sight_[0-9]*_[G-g]lass";

        public override void Setup(GameObject gameObject, EMPController controller)
        {
            _sightPicture = gameObject.RegexFindChild(new Regex(_regex));

            if (_sightPicture == null)
            {
                Logger.Warning("Unable to find sight on {0}!", gameObject.name);
                return;
            }
        }

        protected override void DeviceOff()
        {
            _sightPicture?.SetActive(false);
        }

        protected override void DeviceOn()
        {
            _sightPicture?.SetActive(true);
        }

        protected override void FlickerDevice()
        {
            _sightPicture?.SetActive(FlickerUtil());
        }
    }
}