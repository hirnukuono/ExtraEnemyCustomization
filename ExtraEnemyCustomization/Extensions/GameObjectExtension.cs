using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EECustom
{
    public static class GameObjectExtension
    {
        [SuppressMessage("Type Safety", "UNT0014:Invalid type for call to GetComponent", Justification = "Yes")]
        public static bool TryGetComp<T>(this GameObject obj, out T component)
        {
            //MINOR: Unhollower missing method moment
            component = obj.GetComponent<T>();
            return component != null;
        }

        public static T AddOrGetComponent<T>(this GameObject obj) where T : Component
        {
            if (!TryGetComp(obj, out T comp))
            {
                comp = obj.AddComponent<T>();
            }
            return comp;
        }

        public static GameObject FindChild(this GameObject obj, string name, bool includeInactive = false)
        {
            var comps = obj.GetComponentsInChildren<Transform>(includeInactive);
            foreach (var comp in comps)
            {
                if (comp.gameObject.name != name) continue;
                return comp.gameObject;
            }
            return null;
        }

        public static GameObject RegexFindChild(this GameObject obj, Regex rx, bool includeInactive = false)
        {
            var comps = obj.GetComponentsInChildren<Transform>(includeInactive);
            foreach (var comp in comps)
            {
                if (!rx.IsMatch(comp.name)) continue;
                return comp.gameObject;
            }
            return null;
        }

        public static GameObject Instantiate(this GameObject obj, Transform toParent, string name)
        {
            var newObj = GameObject.Instantiate(obj);
            newObj.transform.parent = toParent;
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localRotation = Quaternion.Euler(Vector3.zero);
            newObj.name = name;
            return newObj;
        }
    }
}