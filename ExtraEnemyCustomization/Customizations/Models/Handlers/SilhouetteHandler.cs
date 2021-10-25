using EECustom.Attributes;
using EECustom.Events;
using Enemies;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace EECustom.Customizations.Models.Handlers
{
    [InjectToIl2Cpp]
    public class EnemySilhouette : MonoBehaviour
    {
        public Material SilhouetteMaterial;

        private Color _latestColorB = Color.clear;

        public EnemySilhouette(IntPtr ptr) : base(ptr)
        {
        }

        public void EnableSilhouette()
        {
            SetColorB(_latestColorB);
        }

        public void DisableSilhouette()
        {
            SetColorB(Color.clear);
        }

        public void SetColorA(Color color)
        {
            SilhouetteMaterial.SetVector("_ColorA", color);
        }

        public void SetColorB(Color color)
        {
            SilhouetteMaterial.SetVector("_ColorB", color);
            _latestColorB = color;
        }
    }

    [InjectToIl2Cpp]
    public class SilhouetteHandler : MonoBehaviour
    {
        public EnemyAgent OwnerAgent;
        public Color DefaultColor;
        public bool RequireTag;
        public bool ReplaceColorWithMarker;
        public bool KeepOnDead;
        public float DeadEffectDelay;

        private bool _tagUpdateDone = true;
        private NavMarker _enemyMarker = null;
        private EnemySilhouette[] _silhouettes = null;

        public SilhouetteHandler(IntPtr ptr) : base(ptr)
        {
        }

        internal void Start()
        {
            _silhouettes = GetComponentsInChildren<EnemySilhouette>(true);
            foreach (var sil in _silhouettes)
            {
                var renderer = sil.GetComponent<Renderer>();
                sil.SilhouetteMaterial = renderer.material;

                if (renderer.shadowCastingMode == ShadowCastingMode.ShadowsOnly || renderer.shadowCastingMode == ShadowCastingMode.Off)
                {
                    renderer.forceRenderingOff = false;
                    renderer.shadowCastingMode = ShadowCastingMode.Off;

                    var culler = OwnerAgent.MovingCuller.Culler;
                    culler.Renderers.Add(renderer);
                    renderer.enabled = true;
                }
            }

            if (RequireTag || ReplaceColorWithMarker)
            {
                EnemyMarkerEvents.RegisterOnMarked(OwnerAgent, OnMarked);
            }

            if (!RequireTag)
            {
                SetColorB(DefaultColor);
                Show();
            }
        }

        internal void Update()
        {
            if (!OwnerAgent.Alive)
            {
                if (!KeepOnDead)
                {
                    KillSilhouette(DeadEffectDelay);
                    enabled = false;
                }
            }

            if (!_tagUpdateDone)
            {
                if (!OwnerAgent.IsTagged)
                {
                    if (RequireTag)
                    {
                        Hide();
                    }

                    _tagUpdateDone = true;
                }

                var alpha = _enemyMarker?.m_enemySubObj?.m_sprites?[0]?.color.a ?? 0.0f;
                var tagColor = _enemyMarker?.m_enemySubObj?.m_sprites?[0]?.color ?? DefaultColor;
                if (RequireTag)
                {
                    var newColor = ReplaceColorWithMarker ? tagColor.AlphaMultiplied(alpha) : DefaultColor.AlphaMultiplied(alpha);
                    SetColorB(newColor);
                }
                else
                {
                    var newColor = Color.Lerp(DefaultColor, tagColor, alpha);
                    SetColorB(newColor);
                }
            }
        }

        public void OnMarked(EnemyAgent agent, NavMarker marker)
        {
            _enemyMarker = marker;
            _tagUpdateDone = false;

            if (RequireTag)
            {
                Show();
            }
        }

        public void KillSilhouette(float delay)
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                CoroutineManager.BlinkOut(_silhouettes[i].gameObject, delay);
            }
        }

        public void Show()
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                _silhouettes[i].EnableSilhouette();
            }
        }

        public void Hide()
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                _silhouettes[i].DisableSilhouette();
            }
        }

        public void SetColorA(Color color)
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                _silhouettes[i].SetColorA(color);
            }
        }

        public void SetColorB(Color color)
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                _silhouettes[i].SetColorB(color);
            }
        }
    }
}