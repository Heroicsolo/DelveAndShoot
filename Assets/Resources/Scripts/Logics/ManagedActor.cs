using Heroicsolo.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using UnityEngine;

namespace Heroicsolo.Logics
{
    internal abstract class ManagedActor : MonoBehaviour, IActor
    {


        [Inject] ActionManager actionManager;

        public void Do(string action, Dictionary<string, object> bag = null)
        {
            GetAction(action).Invoke(this, bag);
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        IEnumerable<IActor.Action> IActor.GetActions()
        {
            return actionManager.GetActions(GetType());
        }

        public IActor.Action GetAction(string name)
        {
            return actionManager.GetAction(GetType(), name);
        }
    }
}
