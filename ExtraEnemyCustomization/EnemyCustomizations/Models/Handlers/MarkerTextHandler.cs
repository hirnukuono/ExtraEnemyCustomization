using EEC.Attributes;
using EEC.Networking;
using EEC.Utils.Unity;
using Enemies;
using Il2CppInterop.Runtime.Attributes;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class MarkerTextHandler : MonoBehaviour
    {
        public EnemyAgent Agent;
        public NavMarker Marker;
        public HealthBarFormat.Worker Worker;
        private string _baseText;
        private bool[] _hasFormat = null;
        private bool _shouldUpdateRainbow = false;
        private Color _rainbow;

        private static readonly MarkerFormatText[] _valuesOfEnum = null;
        private static readonly string[] _formatString = null;
        private static readonly WaitForSeconds _updateYielder = WaitFor.Seconds[0.15f];

        static MarkerTextHandler()
        {
            _valuesOfEnum = Enum.GetValues(typeof(MarkerFormatText)).Cast<MarkerFormatText>().ToArray();
            _formatString = new string[_valuesOfEnum.Length];

            for (int i = 0; i < _valuesOfEnum.Length; i++)
            {
                _formatString[i] = $"[{Enum.GetName(typeof(MarkerFormatText), i)}]".ToUpper();
            }
        }

        internal static bool TextContainsAnyFormat(string baseText)
        {
            for (int i = 1 /*Skips None*/; i < _valuesOfEnum.Length; i++)
            {
                if (baseText.InvariantContains(_formatString[i], ignoreCase: true))
                {
                    return true;
                }
            }

            return false;
        }

        [HideFromIl2Cpp]
        internal void ChangeBaseText(string baseText)
        {
            _baseText = baseText;
            _hasFormat = new bool[_valuesOfEnum.Length];
            var hasAnyFormatText = false;
            for (int i = 1 /*Skips None*/; i < _valuesOfEnum.Length; i++)
            {
                if (_baseText.InvariantContains(_formatString[i], ignoreCase: true))
                {
                    switch ((MarkerFormatText)i)
                    {
                        case MarkerFormatText.NAME:
                            _baseText = _baseText.Replace(_formatString[i], Agent.EnemyData.name, StringComparison.OrdinalIgnoreCase);
                            _hasFormat[i] = false;
                            break;

                        case MarkerFormatText.GAMING:
                            _baseText = _baseText.Replace(_formatString[i], _formatString[i], StringComparison.OrdinalIgnoreCase);
                            _hasFormat[i] = true;
                            hasAnyFormatText = true;
                            _shouldUpdateRainbow = true;
                            break;

                        default:
                            _baseText = _baseText.Replace(_formatString[i], _formatString[i], StringComparison.OrdinalIgnoreCase);
                            _hasFormat[i] = true;
                            hasAnyFormatText = true;
                            break;
                    }
                }
                else
                {
                    _hasFormat[i] = false;
                }
            }

            if (!hasAnyFormatText)
            {
                Marker.SetTitle(_baseText);
                Destroy(this);
            }
        }

        private void OnEnable()
        {
            StopAllCoroutines();
            if (_shouldUpdateRainbow)
            {
                this.StartCoroutine(GamingMoment());
            }
            this.StartCoroutine(UpdateText());
        }

        [HideFromIl2Cpp]
        private IEnumerator UpdateText()
        {
            var oldText = string.Empty;
            var textBuilder = new StringBuilder(_baseText);

            while (true)
            {
                NetworkManager.EnemyHealthState.TryGetState(Agent.GlobalID, out var healthState);
                var maxHealth = healthState.maxHealth;
                var health = healthState.health;

                for (int i = 1 /*Skips None*/; i < _valuesOfEnum.Length; i++)
                {
                    if (!_hasFormat[i])
                        continue;

                    string replace = (MarkerFormatText)i switch
                    {
                        MarkerFormatText.None => string.Empty,
                        MarkerFormatText.NAME => string.Empty,
                        MarkerFormatText.HP => health.ToString("0.00"),
                        MarkerFormatText.HP_ROUND => Mathf.RoundToInt(health).ToString(),
                        MarkerFormatText.HP_MAX => maxHealth.ToString("0.00"),
                        MarkerFormatText.HP_MAX_ROUND => Mathf.RoundToInt(maxHealth).ToString(),
                        MarkerFormatText.HP_PERCENT => (health / maxHealth * 100.0f).ToString("0.00"),
                        MarkerFormatText.HP_PERCENT_ROUND => Mathf.RoundToInt(health / maxHealth * 100.0f).ToString(),
                        MarkerFormatText.HP_BAR => Worker.BuildString(maxHealth, health),
                        MarkerFormatText.GAMING => ColorUtility.ToHtmlStringRGB(_rainbow),
                        _ => string.Empty,
                    };

                    if (!string.IsNullOrEmpty(replace))
                    {
                        textBuilder.Replace(_formatString[i], replace);
                    }
                    yield return null;
                }

                var newText = textBuilder.ToString();
                if (!oldText.InvariantEquals(newText))
                {
                    Marker.SetTitle(newText);
                    oldText = newText;
                }

                textBuilder.Clear();
                textBuilder.Append(_baseText);
                yield return _updateYielder;
            }
        }

        [HideFromIl2Cpp]
        private IEnumerator GamingMoment()
        {
            while (true)
            {
                _rainbow = Color.HSVToRGB(Mathf.Repeat(Clock.ExpeditionProgressionTime, 1.0f), 1.0f, 1.0f);
                yield return null;
            }
        }

        private void OnDestroy()
        {
            Agent = null;
            Marker = null;
            Worker = null;
            _baseText = null;
            _hasFormat = null;
        }
    }

    public enum MarkerFormatText
    {
        None,
        NAME,
        HP,
        HP_ROUND,
        HP_MAX,
        HP_MAX_ROUND,
        HP_PERCENT,
        HP_PERCENT_ROUND,
        HP_BAR,
        GAMING //yes
    }
}