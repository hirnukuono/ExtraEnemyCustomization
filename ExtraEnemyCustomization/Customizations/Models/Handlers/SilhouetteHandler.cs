using EECustom.Attributes;
using EECustom.Events;
using Enemies;
using UnhollowerBaseLib.Attributes;
using UnityEngine;
using UnityEngine.Rendering;

namespace EECustom.Customizations.Models.Handlers
{
    [InjectToIl2Cpp]
    internal sealed class EnemySilhouette : MonoBehaviour
    {
        public Material SilhouetteMaterial;

        private Color _latestColorB = Color.clear;

        internal void OnDestroy()
        {
            SilhouetteMaterial = null;
        }

        [HideFromIl2Cpp]
        public void EnableSilhouette()
        {
            SetColorB(_latestColorB);
        }

        [HideFromIl2Cpp]
        public void DisableSilhouette()
        {
            SetColorB(Color.clear);
        }

        [HideFromIl2Cpp]
        public void SetColorA(Color color)
        {
            SilhouetteMaterial.SetVector("_ColorA", color);
        }

        [HideFromIl2Cpp]
        public void SetColorB(Color color)
        {
            SilhouetteMaterial.SetVector("_ColorB", color);
            _latestColorB = color;
        }
    }

    [InjectToIl2Cpp]
    internal sealed class SilhouetteHandler : MonoBehaviour
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
        private bool _eventRegistered = false;

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
                EnemyMarkerEvents.Marked += OnMarked;
                _eventRegistered = true;
            }

            if (!RequireTag)
            {
                SetColorB(DefaultColor);
                Show();
            }
        }

        internal void OnDestroy()
        {
            if (_eventRegistered)
                EnemyMarkerEvents.Marked -= OnMarked;

            OwnerAgent = null;
            _enemyMarker = null;
            _silhouettes = null;
        }

        internal void FixedUpdate()
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

                float alpha = 0.0f;
                Color tagColor = DefaultColor;
                if (TryGetEnemyMarkerSpriteRenderer(out var renderer))
                {
                    alpha = renderer.color.a;
                    tagColor = renderer.color;
                }

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

        [HideFromIl2Cpp]
        private void OnMarked(EnemyAgent agent, NavMarker marker)
        {
            if (agent.GlobalID != OwnerAgent.GlobalID)
                return;

            _enemyMarker = marker;
            _tagUpdateDone = false;

            if (RequireTag)
            {
                Show();
            }
        }

        [HideFromIl2Cpp]
        public void KillSilhouette(float delay)
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                CoroutineManager.BlinkOut(_silhouettes[i].gameObject, delay);
            }
        }

        [HideFromIl2Cpp]
        public void Show()
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                _silhouettes[i].EnableSilhouette();
            }
        }

        [HideFromIl2Cpp]
        public void Hide()
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                _silhouettes[i].DisableSilhouette();
            }
        }

        [HideFromIl2Cpp]
        public void SetColorA(Color color)
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                _silhouettes[i].SetColorA(color);
            }
        }

        [HideFromIl2Cpp]
        public void SetColorB(Color color)
        {
            for (int i = 0; i < _silhouettes.Length; i++)
            {
                _silhouettes[i].SetColorB(color);
            }
        }

        private bool TryGetEnemyMarkerSpriteRenderer(out SpriteRenderer renderer)
        {
            if (_enemyMarker == null && _enemyMarker.m_enemySubObj == null)
            {
                renderer = null;
                return false;
            }

            var enemySubObj = _enemyMarker.m_enemySubObj;
            if (enemySubObj.m_sprites == null)
            {
                renderer = null;
                return false;
            }

            if (enemySubObj.m_sprites.Length < 1)
            {
                renderer = null;
                return false;
            }

            renderer = enemySubObj.m_sprites[0];
            return renderer != null;
        }
    }
}