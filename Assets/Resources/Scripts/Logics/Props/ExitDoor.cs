using Assets.Resources.Scripts.Logics;
using Heroicsolo.DI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Heroicsolo.Logics.ActionManager;

namespace Heroicsolo.Logics
{
    public class ExitDoor : ManagedActor, IInteractable
    {
        [SerializeField] private bool isInteractable = true;
        [SerializeField] private GameObject openVisuals;

        [Inject] private IActionManager actionManager;
        [Inject] private IGameController gameController;

        internal override IActionManager ActionManager => actionManager;

        public bool IsInteractable => isInteractable;

        [ActorAction]
        private bool Open(Dictionary<string, object> bag = null)
        {
            gameController.LevelCompleted();
            return true;
        }

        public void SetInteractable(bool value)
        {
            isInteractable = value;

            if (openVisuals != null)
            {
                openVisuals.SetActive(value);
            }
        }

        public void Interact()
        {
            if (isInteractable)
                Do("Open");
        }

        IInteractable.InteractionType IInteractable.GetInteractionType()
        {
            return IInteractable.InteractionType.ExitDoor;
        }

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);
            actionManager.RegisterManagedActor(typeof(ExitDoor));
        }
    }
}