using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public partial interface IActor
    {
        void Do(string action, Dictionary<string, object> bag = null);
        void Do(IAction action, Dictionary<string, object> bag = null);
        //IActionManager ActionManager { get; }
        GameObject GetGameObject();
        IEnumerable<IAction> GetActions(); //Get actions dict from manager to modify actoins of actor
        IAction GetAction(string name);
        T GetComponent<T>();
    }
}
