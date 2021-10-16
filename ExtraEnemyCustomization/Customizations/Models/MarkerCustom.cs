using EECustom.Events;
using EECustom.Extensions;
using EECustom.Managers;
using Enemies;
using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace EECustom.Customizations.Models
{
    public class MarkerCustom : EnemyCustomBase, IEnemyPrefabBuiltEvent, IEnemySpawnedEvent
    {
        public string SpriteName { get; set; } = string.Empty;
        public Color MarkerColor { get; set; } = new Color(0.8235f, 0.1843f, 0.1176f);
        public string MarkerText { get; set; } = string.Empty;
        public bool BlinkIn { get; set; } = false;
        public bool Blink { get; set; } = false;
        public float BlinkDuration { get; set; } = 30.0f;
        public float BlinkMinDelay { get; set; } = 1.0f;
        public float BlinkMaxDelay { get; set; } = 5.0f;
        public bool AllowMarkingOnHibernate { get; set; } = false;

        private Sprite _Sprite = null;
        private bool _PrespawnOnce = false;
        private bool _HasText = false;
        private bool _TextRequiresAutoUpdate = false;

        public override string GetProcessName()
        {
            return "Marker";
        }

        public override void OnConfigLoaded()
        {
            //TODO: Implement it someday
            if (string.IsNullOrEmpty(MarkerText))
                return;

            _HasText = true;

            if (MarkerText.ContainsAnyIgnoreCase("[HP_MAX]", "[HP]", "[HP_PERCENT]", "[HP_PERCENT_INT]"))
            {
                _TextRequiresAutoUpdate = true;
            }
        }

        public void OnPrefabBuilt(EnemyAgent agent)
        {
            if (!_PrespawnOnce)
            {
                _PrespawnOnce = true;

                if (string.IsNullOrEmpty(SpriteName))
                    return;

                if (!SpriteManager.TryGetSpriteCache(SpriteName, 64.0f, out _Sprite))
                    _Sprite = SpriteManager.GenerateSprite(SpriteName);
            }
        }

        public void OnSpawned(EnemyAgent agent)
        {
            if (AllowMarkingOnHibernate)
                agent.ScannerData.m_soundIndex = 0; //I know... this is such a weird way to do it...

            EnemyMarkerEvents.RegisterOnMarked(agent, OnMarked);
        }

        private void OnMarked(EnemyAgent agent, NavMarker marker)
        {
            marker.m_enemySubObj.SetColor(MarkerColor);
            //marker.SetTitle("wew");
            //marker.SetVisualStates(NavMarkerOption.Enemy | NavMarkerOption.Title, NavMarkerOption.Enemy | NavMarkerOption.Title, NavMarkerOption.Empty, NavMarkerOption.Empty);
            //MINOR: Adding Text for Marker maybe?

            if (_Sprite != null)
            {
                var renderer = marker.m_enemySubObj.GetComponentInChildren<SpriteRenderer>();
                renderer.sprite = _Sprite;
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