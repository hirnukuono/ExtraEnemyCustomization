using System;
using System.Collections.Generic;
using System.Text;

namespace EECustom.Utils
{
    public static class ListExtension
    {
        public static void ForEachFromBackAndClear<T>(this List<T> list, Action<T> action)
        {
            if (list == null)
                return;

            if (list.Count < 1)
                return;

            if (list.Count > 1)
                list.Reverse();

            list.ForEach(action);
            list.Clear();
        }
    }
}
