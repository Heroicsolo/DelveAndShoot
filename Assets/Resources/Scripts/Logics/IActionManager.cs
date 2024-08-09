using Heroicsolo.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Heroicsolo.Logics
{
    internal interface IActionManager : ISystem
    {
        class Action<T> where T : IActor
        {
            public delegate bool Act(T actor, Dictionary<string, object> bag = null);

            List<Act> preActions;
            Act action;
            List<Act> postActions;
            public event Act OnAction;

            public void Invoke(T actor, Dictionary<string, object> bag = null)
            {
                bool success = preActions.All(pa => pa.Invoke(actor, bag));
                if (success) success = action.Invoke(actor, bag);
                if (success) postActions.ForEach(pa => pa.Invoke(actor, bag));
                OnAction.BeginInvoke(actor, bag, null, null); //Maybe exception
            }

            public void AddPreAction(Act action) { preActions.Add(action); }
            public void RemovePreAction(Act action) { preActions.Remove(action); }
            public void AddPostAction(Act action) { postActions.Add(action); }
            public void RemovePostAction(Act action) { postActions.Remove(action); }
        }
        Action RegisterAction(Type actorType, string name);
        void Do(IActor actor, string name);
        IActionManager.Action<T> GetAction<T>(string name) where T : IActor;
        //Register action (actor Type + unique name)
    }
}
