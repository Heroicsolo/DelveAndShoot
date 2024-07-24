using Assets.Resources.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Heroicsolo.Utils
{

    public class AutohidingTarget : MonoBehaviour
    {
        [SerializeField]
        private Camera visibleFromCamera;
        [SerializeField]
        [Min(0)]
        private float hidingRadius = 1;
        [SerializeField]
        float ignoreOffset = 1;
        private List<IHideable> hidedObjects = new List<IHideable>();

        void Start()
        {
            if (visibleFromCamera == null)
                visibleFromCamera = Camera.main;
        }

        float CalcFadeCoef()
        {
            Vector3 edgePoint = transform.position + visibleFromCamera.transform.right * hidingRadius;
            var myDir = (transform.position - visibleFromCamera.transform.position).normalized;
            var edgeDir = (edgePoint - visibleFromCamera.transform.position).normalized;
            float maxDelta = Vector3.Cross(edgeDir, myDir).magnitude;
            return 1 / maxDelta;
        }

        float CalcDelta(Vector3 other)
        {
            var otherDir = (other - visibleFromCamera.transform.position).normalized;
            var myDir = (transform.position - visibleFromCamera.transform.position).normalized;
            return Vector3.Cross(otherDir, myDir).magnitude;
        }

        Dictionary<IHideable, float> GetHideblesOnWay()
        {
            var cameraPos = visibleFromCamera.transform.position;
            var targetDirection = (transform.position - cameraPos);
            RaycastHit[] castRes = Physics.CapsuleCastAll(
                cameraPos,
                transform.position,
                hidingRadius,
                targetDirection.normalized,
                targetDirection.magnitude-ignoreOffset);

            float fadeCoef = CalcFadeCoef();
            var kvs = castRes
                .Select(i => new KeyValuePair<IHideable, float>(
                    i.collider.GetComponent<IHideable>(),
                    CalcDelta(i.transform.position) * fadeCoef
                    )
                )
                .Where(i => i.Key != null);

            return new Dictionary<IHideable, float>(kvs);
        }

        void UpdateOldHided(Dictionary<IHideable, float> newHided)
        {
            var toUnhide = hidedObjects.Except(newHided.Keys);
            foreach (var hideable in toUnhide)
            {
                hideable.Unhide();
                hidedObjects.Remove(hideable);
            }
        }

        // Update is called once per frame
        void Update()
        {
            var hideblesOnWay = GetHideblesOnWay();

            UpdateOldHided(hideblesOnWay);

            foreach (var (hideble, delta) in hideblesOnWay)
            {
                hideble.Hide(delta);
                if (!hidedObjects.Contains(hideble))
                    hidedObjects.Add(hideble);
            }
        }
    }
}