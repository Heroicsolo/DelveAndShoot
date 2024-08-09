using Heroicsolo.DI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroicsolo.Logics
{
    internal interface IActionManager : ISystem
    {

        void RegisterManagedActor(System.Type actorType);
        //Action<IActor> RegisterAction<T>(IActor.Action action) where T:IActor;
        //void Do(IActor actor, string name);
        IActor.Action GetAction(System.Type actorType, string name);
        IActor.Action GetAction<T>(string name) where T : ManagedActor;
        //Register action (actor Type + unique name)
    }
}
