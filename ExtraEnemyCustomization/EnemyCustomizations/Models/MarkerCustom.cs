using EEC.EnemyCustomizations.Models.Handlers;
using EEC.Events;
using EEC.Managers.Assets;
using EEC.Utils;
using Enemies;
using System;
using System.Text;
using UnityEngine;

namespace EEC.EnemyCustomizations.Models
{
    public sealed class MarkerCustom : EnemyCustomBase, IEnemySpawnedEvent
    {
        public string SpriteName { get; set; } = string.Empty;
        public Color MarkerColor { get; set; } = new Color(0.8235f, 0.1843f, 0.1176f);
        public string MarkerText { get; set; } = string.Empty;
        public HealthBarFormat MarkerTextHealthBarFormat { get; set; } = new();
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
            if (Configuration.ShowMarkerText)
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

            if (!Configuration.ShowMarkerDistance)
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
                        handler.Worker = MarkerTextHealthBarFormat.CreateWorker();
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
                    float time = 0.4f + Rand.Range(BlinkMinDelay, BlinkMaxDelay);
                    for (; time <= duration; time += Rand.Range(BlinkMinDelay, BlinkMaxDelay))
                    {
                        CoroutineManager.BlinkIn(marker.m_enemySubObj.gameObject, time);
                    }
                }
            }
        }
    }

    public sealed class HealthBarFormat
    {
        public int Count { get; set; } = 8;
        public string FilledBarText { get; set; } = "|";
        public string EmptyBarText { get; set; } = " ";

        public Worker CreateWorker()
        {
            return new Worker()
            {
                Count = Count,
                FilledBarText = FilledBarText,
                EmptyBarText = EmptyBarText
            };
        }

        public sealed class Worker
        {
            public int Count;
            public string FilledBarText;
            public string EmptyBarText;

            private StringBuilder _stringBuilder;
            private int _lastFilledBar = -1;
            private string _lastBarText = string.Empty;

            public string BuildString(float maxHealth, float health)
            {
                var filledBarCount = Mathf.RoundToInt(Mathf.Lerp(0, Count, health / maxHealth));
                if (filledBarCount == _lastFilledBar)
                {
                    return _lastBarText;
                }

                _lastFilledBar = filledBarCount;
                if (_stringBuilder == null)
                {
                    _stringBuilder = new();
                }
                else
                {
                    _stringBuilder.Clear();
                }

                for (int i = 0; i < Count; i++)
                {
                    if (i < filledBarCount)
                    {
                        _stringBuilder.Append(FilledBarText);
                    }
                    else
                    {
                        _stringBuilder.Append(EmptyBarText);
                    }
                }

                _lastBarText = _stringBuilder.ToString();
                return _lastBarText;
            }
        }
    }
}