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
        public IActionManager ActionManager => actionManager;

        public void Do(string action)
        {
            GetAction(action).Invoke(this);
        }

        public GameObject GetGameObject()
        {
            throw new NotImplementedException();
        }

        List<IActor.Action> IActor.GetActions()
        {
            throw new NotImplementedException();
        }

        public IActor.Action GetAction(string name)
        {
            throw new NotImplementedException();
        }
    }
}
