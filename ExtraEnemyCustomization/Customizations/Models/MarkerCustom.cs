using EECustom.Customizations.Models.Handlers;
using EECustom.Events;
using EECustom.Managers.Assets;
using Enemies;
using System;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public sealed class MarkerCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public string SpriteName { get; set; } = string.Empty;
        public Color MarkerColor { get; set; } = new Color(0.8235f, 0.1843f, 0.1176f);
        public string MarkerText { get; set; } = string.Empty;
        public bool ShowDistance { get; set; } = false;
        public bool BlinkIn { get; set; } = false;
        public bool Blink { get; set; } = false;
        public float BlinkDuration { get; set; } = 30.0f;
        public float BlinkMinDelay { get; set; } = 1.0f;
        public float BlinkMaxDelay { get; set; } = 5.0f;
        public bool AllowMarkingOnHibernate { get; set; } = false;

        private Sprite _sprite = null;
        private bool _hasText = false;
        private bool _textRequiresAutoUpdate = false;

        public override string GetProcessName()
        {
            return "Marker";
        }

        public override void OnAssetLoaded()
        {
            if (string.IsNullOrEmpty(SpriteName))
                return;

            if (!SpriteManager.TryGetSpriteCache(SpriteName, 64.0f, out _sprite))
                _sprite = SpriteManager.GenerateSprite(SpriteName);
        }

        public override void OnConfigLoaded()
        {
            if (Configuration.ShowMarkerText.Value == true)
            {
                if (!string.IsNullOrEmpty(MarkerText))
                {
                    _hasText = true;

                    if (MarkerTextHandler.TextContainsAnyFormat(MarkerText))
                    {
                        _textRequiresAutoUpdate = true;
                    }
                }
            }

            if (Configuration.ShowMarkerDistance.Value == false)
            {
                ShowDistance = false;
            }

            EnemyMarkerEvents.Marked += OnMarked;
        }

        public override void OnConfigUnloaded()
        {
            EnemyMarkerEvents.Marked -= OnMarked;
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (AllowMarkingOnHibernate)
                agent.ScannerData.m_soundIndex = 0; //I know... this is such a weird way to do it...
        }

        private void OnMarked(EnemyAgent agent, NavMarker marker)
        {
            if (!IsTarget(agent))
                return;

            marker.m_enemySubObj.SetColor(MarkerColor);

            var option = NavMarkerOption.Enemy;
            if (_hasText)
            {
                option |= NavMarkerOption.Title;

                if (_textRequiresAutoUpdate)
                {
                    if (!marker.gameObject.TryGetComp<MarkerTextHandler>(out var handler))
                    {
                        handler = marker.gameObject.AddComponent<MarkerTextHandler>();
                        handler.Agent = agent;
                    }

                    handler.Marker = marker;
                    handler.ChangeBaseText(MarkerText);
                }
                else
                {
                    marker.SetTitle(MarkerText);
                }
            }

            if (ShowDistance)
            {
                option |= NavMarkerOption.Distance;
            }

            marker.SetVisualStates(option, option, NavMarkerOption.Empty, NavMarkerOption.Empty);

            if (_sprite != null)
            {
                var renderer = marker.m_enemySubObj.GetComponentInChildren<SpriteRenderer>();
                renderer.sprite = _sprite;
            }

            if (BlinkIn)
            {
                CoroutineManager.BlinkIn(marker.m_enemySubObj.gameObject, 0.0f);
                CoroutineManager.BlinkIn(marker.m_enemySubObj.gameObject, 0.2f);
            }

            if (Blink)
            {
                if (BlinkMinDelay >= 0.0f && BlinkMinDelay < BlinkMaxDelay)
                {
                    float duration = Math.Min(BlinkDuration, agent.EnemyBalancingData.TagTime);
                    float time = 0.4f + UnityEngine.Random.RandomRange(BlinkMinDelay, BlinkMaxDelay);
                    for (; time <= duration; time += UnityEngine.Random.RandomRange(BlinkMinDelay, BlinkMaxDelay))
                    {
                        CoroutineManager.BlinkIn(marker.m_enemySubObj.gameObject, time);
                    }
                }
            }
        }
    }
}