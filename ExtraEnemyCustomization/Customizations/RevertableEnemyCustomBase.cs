using System;
using System.Collections.Generic;

namespace EECustom.Customizations
{
    [Obsolete("Enemy Prefab will now be regenerated after reload!")]
    public abstract class RevertableEnemyCustomBase : EnemyCustomBase
    {
        private static readonly Stack<Action> _revertDelegates = new();

        public static void PushRevertJob(Action revertAction)
        {
            _revertDelegates.Push(revertAction);
        }

        public override sealed void OnConfigUnloaded()
        {
            while (_revertDelegates.Count > 0)
            {
                var action = _revertDelegates.Pop();
                action?.Invoke();
            }

            OnConfigUnloadedPost();
        }

        public virtual void OnConfigUnloadedPost()
        {
        }
    }
}