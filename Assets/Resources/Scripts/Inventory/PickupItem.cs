using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.DI;
using Heroicsolo.Scripts.Logics;
using Heroicsolo.Scripts.Player;
using Heroicsolo.Scripts.UI;
using Heroicsolo.Utils;
using Naxeex.Content_dev.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Inventory
{
    public class PickupItem : MonoBehaviour, IPooledObject
    {
        private const float PickupFlightTime = 1f;
        private const float FlyHeight = 2f;

        [SerializeField] private ItemId itemId;
        [SerializeField] [Min(1)] private int amount;
        [SerializeField] private PickupMode pickupMode;

        [Inject] private IGameUIController gameUIController;

        private bool readyToPickup;
        private bool isFlying;
        private PlayerController playerController;

        public void FlyToPoint(Vector3 point)
        {
            isFlying = true;
            StartCoroutine(FlyToPointAnim(point));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isFlying && other.TryGetComponent(out playerController))
            {
                if (pickupMode == PickupMode.Auto)
                {
                    PickUp();
                }
                else
                {
                    readyToPickup = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out _))
            {
                readyToPickup = false;
            }
        }

        private void PickUp()
        {
            isFlying = true;
            gameUIController.SetUIElementSelected(false);
            gameUIController.SetCursorState(CursorState.Default, true);
            StartCoroutine(PickupAnim());
        }

        private IEnumerator FlyToPointAnim(Vector3 point)
        {
            float t = 0;

            Vector3 startPos = transform.position;
            Vector3 midPos = (point + startPos) / 2f + FlyHeight * Vector3.up;

            do
            {
                t += Time.deltaTime;

                transform.position = BezierCurve.GetCurvedPosition(t / PickupFlightTime, startPos, midPos, point);

                yield return null;
            }
            while (t < PickupFlightTime);

            isFlying = false;
        }

        private IEnumerator PickupAnim()
        {
            Transform playerTransform = playerController.transform;

            float t = 0;

            Vector3 startPos = transform.position;

            do
            {
                t += Time.deltaTime;

                transform.position = Vector3.Lerp(startPos, playerTransform.position, t / PickupFlightTime);

                yield return null;
            }
            while (t < PickupFlightTime);

            //TODO: add item to inventory

            PoolSystem.ReturnToPool(this);
        }

        private void OnEnable()
        {
            isFlying = false;
        }

        private void OnMouseEnter()
        {
            if (!isFlying && pickupMode == PickupMode.Click)
            {
                gameUIController.SetUIElementSelected(true);
                gameUIController.SetCursorState(CursorState.PickUp, true);
            }
        }

        private void OnMouseExit()
        {
            if (pickupMode == PickupMode.Click)
            {
                gameUIController.SetUIElementSelected(false);
                gameUIController.SetCursorState(CursorState.Default, true);
            }
        }

        private void OnMouseUpAsButton()
        {
            if (!isFlying && pickupMode == PickupMode.Click && readyToPickup)
            {
                PickUp();
            }
        }

        public string GetName()
        {
            return gameObject.name;
        }

        public void SetName(string name)
        {
            gameObject.name = name;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }

    [Serializable]
    public enum PickupMode
    {
        Auto = 0,
        Click = 1
    }
}