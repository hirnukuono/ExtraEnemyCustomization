using EECustom.CustomAbilities.Explosion.Handlers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EECustom.CustomAbilities.Explosion
{
    public static class ExplosionEffectPooling
    {
        private static readonly Queue<ExplosionEffectHandler> _pool = new();

        public static void Initialize()
        {
            for (int i = 0; i < 30; i++)
            {
                _pool.Enqueue(CreatePoolObject());
            }
        }

        private static ExplosionEffectHandler CreatePoolObject()
        {
            var newObject = new GameObject();
            UnityEngine.Object.DontDestroyOnLoad(newObject);

            var effectHandler = newObject.AddComponent<ExplosionEffectHandler>();
            newObject.SetActive(false);
            return effectHandler;
        }

        public static void TryDoEffect(ExplosionEffectData data)
        {
            if(_pool.TryDequeue(out var handler))
            {
                handler.EffectDoneOnce = () =>
                {
                    handler.gameObject.SetActive(false);
                    _pool.Enqueue(handler);
                };
                handler.gameObject.SetActive(true);
                handler.DoEffect(data);
            }
        }
    }

    public struct ExplosionEffectData
    {
        public Vector3 position;
        public Color flashColor;
        public float intensity;
        public float range;
        public float duration;
    }
}
