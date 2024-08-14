using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Resources.Scripts.Logics
{
    internal interface IInteractable
    {
        enum InteractionType
        {
            Torch
        }
        void Interact();
        InteractionType GetInteractionType();
        bool IsInteractable { get; }
    }
}
