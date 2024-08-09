using Heroicsolo.DI;
using System.Collections.Generic;

namespace Heroicsolo.Logics
{
    internal interface IActionManager : ISystem
    {

        void RegisterManagedActor(System.Type actorType);
        //Action<IActor> RegisterAction<T>(IActor.Action action) where T:IActor;
        //void Do(IActor actor, string name);
        IEnumerable<IActor.Action> GetActions(System.Type actorType);
        IActor.Action GetAction(System.Type actorType, string name);
        IActor.Action GetAction<T>(string name) where T : ManagedActor;
        //Register action (actor Type + unique name)
    }
}
