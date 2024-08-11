using Heroicsolo.DI;
using System.Collections.Generic;

namespace Heroicsolo.Logics
{
    internal interface IActionManager : ISystem
    {

        void RegisterManagedActor(System.Type actorType);
        //Action<IActor> RegisterAction<T>(IActor.Action action) where T:IActor;
        //void Do(IActor actor, string name);
        IEnumerable<IAction> GetActions(System.Type actorType);
        IAction GetAction(System.Type actorType, string name);
        IAction GetAction<T>(string name) where T : ManagedActor;
        //Register action (actor Type + unique name)
    }
}
