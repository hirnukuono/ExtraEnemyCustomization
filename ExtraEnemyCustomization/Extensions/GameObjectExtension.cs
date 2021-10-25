using UnityEngine;

namespace EECustom.Extensions
{
    public static class GameObjectExtension
    {
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