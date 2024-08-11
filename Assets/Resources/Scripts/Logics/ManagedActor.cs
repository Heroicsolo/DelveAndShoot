using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System.Xml.Schema;
using UnityEditor;

namespace Heroicsolo.Logics
{
    public abstract partial class ManagedActor : MonoBehaviour, IActor
    {


        internal abstract IActionManager ActionManager { get; }

        public void Do(string action, Dictionary<string, object> bag = null)
        {
            GetAction(action).Invoke(this, bag);
        }

        public void Do(IAction.ActionModule method, Dictionary<string, object> bag = null)
        {
            string name = method.GetMethodInfo().GetCustomAttribute<ActionManager.ActorActionAttribute>()?.name;
            if(name != null)
                GetAction(name).Invoke(this, bag);
        }
        public void Do(IAction action, Dictionary<string, object> bag = null)
        {
            var manAction = action as ManagedAction;
            bool success = manAction.PreActions.All(pa => pa.Invoke(this, bag));
            if (success) success = InvokeActionBase(manAction);
            if (success) manAction.PostActions.ForEach(pa => pa.Invoke(this, bag));
        }

        Dictionary<ManagedAction, IAction.ActionBase> instancedDelegates = new();
        private bool InvokeActionBase(ManagedAction action, Dictionary<string, object> bag = null)
        {
            if (!instancedDelegates.ContainsKey(action))
            {
                var delegateInstance = action.ActionBase.CreateDelegate(typeof(IAction.ActionBase), this) as IAction.ActionBase;
                instancedDelegates[action] = delegateInstance;
            }
            return instancedDelegates[action].Invoke();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
        IEnumerable<IAction> IActor.GetActions()
        {
            return ActionManager.GetActions(GetType());
        }
        public IAction GetAction(string name)
        {
            return ActionManager.GetAction(GetType(), name);
        }
    }
}
