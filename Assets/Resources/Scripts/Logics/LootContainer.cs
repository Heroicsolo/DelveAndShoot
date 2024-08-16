using Heroicsolo.DI;
using Heroicsolo.Inventory;
using Heroicsolo.Logics;
using Heroicsolo.Heroicsolo.Player;
using Heroicsolo.Scripts.UI;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using Assets.Resources.Scripts.Logics;

namespace Heroicsolo.Logics
{
    public class LootContainer : MonoBehaviour, IInteractable
    {
        [SerializeField] private string lootId;
        [SerializeField] private string title;
        [SerializeField] private PlayableDirector openDirector;
        [SerializeField] private List<GameObject> objectsToHide = new();
        [SerializeField][Min(0f)] private float openRadius = 3f;

        [Inject] private IGameUIController gameUIController;
        [Inject] private ILootManager lootManager;
        [Inject] private IGameController gameController;

        private Guid guid;
        private bool readyToOpen;
        private bool isEmpty;
        private PlayerController playerController;

        public bool IsInteractable => readyToOpen;

        private void OnPlayerEnteredToOpenArea()
        {
            readyToOpen = true;
        }

        private void OnPlayerLeftOpenArea()
        {
            readyToOpen = false;
        }

        private void Open()
        {
            if (openDirector != null)
            {
                openDirector.Play();
            }

            objectsToHide.ForEach(obj => obj.SetActive(false));

            readyToOpen = false;

            isEmpty = true;

            gameUIController.SetUIElementSelected(false);
            gameUIController.SetCursorState(CursorState.Default, true);
            gameUIController.HideWorldItemDesc(guid);

            lootManager.GenerateRandomDrop(lootId, transform.position);
        }

        private void OnMouseEnter()
        {
            if (!isEmpty)
            {
                gameUIController.SetUIElementSelected(true);
                gameUIController.SetCursorState(CursorState.PickUp, true);
                gameUIController.ShowWorldItemDesc(guid, title, transform.position);
            }
        }

        private void OnMouseExit()
        {
            gameUIController.SetUIElementSelected(false);
            gameUIController.SetCursorState(CursorState.Default, true);
            gameUIController.HideWorldItemDesc(guid);
        }

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);

            guid = Guid.NewGuid();

            playerController = gameController.GetPlayerController();

            readyToOpen = false;

            isEmpty = false;
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, playerController.transform.position) < openRadius)
            {
                OnPlayerEnteredToOpenArea();
            }
            else
            {
                OnPlayerLeftOpenArea();
            }

            //if (Input.GetMouseButtonUp(1) && readyToOpen && !isEmpty)
            //{
            //    Open();
            //}
        }

        public void Interact()
        {
            if (readyToOpen && !isEmpty)
            {
                Open();
            }
        }

        IInteractable.InteractionType IInteractable.GetInteractionType()
        {
            return IInteractable.InteractionType.Container;
        }
    }
}