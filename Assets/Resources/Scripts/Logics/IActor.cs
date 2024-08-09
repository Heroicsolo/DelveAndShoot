using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public interface IActor
    {
        public delegate bool Act(IActor actor, Dictionary<string, object> bag = null);
        public class Action
        {

            List<Act> preActions = new();
            Act action;
            List<Act> postActions = new();
            public event Act OnAction;
            public string Name { get; private set; }

            public Action(string name, Act action = null)
            {
                Name = name;
                this.action = action;
            }

            internal void Invoke(IActor actor, Dictionary<string, object> bag = null)
            {
                bool success = preActions.All(pa => pa.Invoke(actor, bag));
                if (success) success = action.Invoke(actor, bag);
                if (success) postActions.ForEach(pa => pa.Invoke(actor, bag));
                if (OnAction != null)
                    OnAction.BeginInvoke(actor, bag, null, null);
            }

            internal void AddPreAction(Act action) { preActions.Add(action); }
            internal void RemovePreAction(Act action) { preActions.Remove(action); }
            internal void AddPostAction(Act action) { postActions.Add(action); }
            internal void RemovePostAction(Act action) { postActions.Remove(action); }
        }

        void Do(string action, Dictionary<string, object> bag);
        //IActionManager ActionManager { get; }
        GameObject GetGameObject();
        IEnumerable<Action> GetActions(); //Get actions dict from manager to modify actoins of actor
        Action GetAction(string name);
    }
}
