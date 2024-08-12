using System.Collections.Generic;

namespace Heroicsolo.Logics
{
    public interface IAction
    {
        public delegate bool ActionModule(IActor actor, Dictionary<string, object> bag = null);
        public delegate bool ActionBase(Dictionary<string, object> bag = null);

        public string Name { get; }
        public void Invoke(IActor actor, Dictionary<string, object> bag = null);
        public void AddPreAction(ActionModule action);
        public void RemovePreAction(ActionModule action);
        public void AddPostAction(ActionModule action);
        public void RemovePostAction(ActionModule action);
        event ActionModule OnAction;
    }
}