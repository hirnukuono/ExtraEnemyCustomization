using System;
using System.Collections.Generic;

namespace EECustom.Customizations
{
    public abstract class RevertableEnemyCustomBase : EnemyCustomBase
    {
        private readonly static Stack<Action> _revertDelegates = new();

        public void PushRevertJob(Action revertAction)
        {
            _revertDelegates.Push(revertAction);
        }

        public sealed override void OnConfigUnloaded()
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
