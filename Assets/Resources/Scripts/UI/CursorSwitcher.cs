using Heroicsolo.DI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Heroicsolo.Scripts.UI
{
    public class CursorSwitcher : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private CursorState cursorMode;

        [Inject] private IGameUIController gameUIController;

        public void OnPointerEnter(PointerEventData eventData)
        {
            gameUIController.SetUIElementSelected(true);
            gameUIController.SetCursorState(cursorMode, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameUIController.SetUIElementSelected(false);
        }

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);
        }
    }
}