using Heroicsolo.DI;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public abstract class ManagedActor : MonoBehaviour, IActor
    {


        internal abstract IActionManager ActionManager { get; }

        public void Do(string action, Dictionary<string, object> bag = null)
        {
            GetAction(action).Invoke(this, bag);
        }

        public void Do(IActor.Act method, Dictionary<string, object> bag = null)
        {
            string name = method.GetMethodInfo().GetCustomAttribute<ActionManager.ActorActionAttribute>()?.name;
            if(name != null)
                GetAction(name).Invoke(this, bag);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
        IEnumerable<IActor.Action> IActor.GetActions()
        {
            return ActionManager.GetActions(GetType());
        }
        public IActor.Action GetAction(string name)
        {
            return ActionManager.GetAction(GetType(), name);
        }
    }
}
