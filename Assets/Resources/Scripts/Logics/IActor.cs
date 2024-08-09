using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroicsolo.Logics
{
    internal interface IActor
    {
        void Do(string action);
        IActionManager ActionManager { get; }
        List<IActionManager.Action<IActor>> GetActions();
    }
}
