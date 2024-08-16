using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Heroicsolo.Logics
{
    public class ManagedAction : IAction
    {
        List<IAction.ActionModule> preActions = new();
        MethodInfo actionBase;
        List<IAction.ActionModule> postActions = new();
        public event IAction.ActionModule OnAction;
        public string Name { get; private set; }

        internal List<IAction.ActionModule> PreActions => preActions;
        internal List<IAction.ActionModule> PostActions => postActions;
        internal MethodInfo ActionBase => actionBase;

        public ManagedAction(string name, IAction.ActionBase action)
        {
            Name = name;
            this.actionBase = action.GetMethodInfo();
            postActions.Add((a, b) => { OnAction?.Invoke(a, b); return true; });
        }
        public ManagedAction(string name, MethodInfo action) //TODO: Add methodInfo validation and conversion for bag-less methods
        {
            Name = name;
            this.actionBase = action;
            postActions.Add((a, b) => { OnAction?.Invoke(a, b); return true; });
        }

        public void Invoke(IActor actor, Dictionary<string, object> bag = null)
        {
            var manActor = actor as ManagedActor;
            manActor.Do(this, bag);
        }

        public void AddPreAction(IAction.ActionModule action) { preActions.Add(action); }
        public void RemovePreAction(IAction.ActionModule action) { preActions.Remove(action); }
        public void AddPostAction(IAction.ActionModule action) { postActions.Add(action); }
        public void RemovePostAction(IAction.ActionModule action) { postActions.Remove(action); }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}

