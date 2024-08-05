using Heroicsolo.DI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Heroicsolo.Scripts.UI
{
    public interface IDialogPopup : ISystem, IPointerClickHandler
    {
        void ShowMessage(string message, Sprite avatarSprite);
    }
}