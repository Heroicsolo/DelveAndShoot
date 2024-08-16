using Heroicsolo.Heroicsolo.Player;
using System;
using UnityEngine;

namespace Heroicsolo.Logics.Traps
{
    public class Trap : MonoBehaviour
    {
        [Header("Balance")]
        [SerializeField] private TrapActivationType activationType;
        [SerializeField] private TrapWorkMode workMode;
        [ConditionalHide("workMode", false, TrapWorkMode.Endless)]
        [SerializeField] [Min(0f)] private float lifeTime = 3f;
        [ConditionalHide("workMode", true, TrapWorkMode.Periodical)]
        [SerializeField] [Min(0f)] private float activationPeriod = 10f;

        [Header("Trap Parts")]
        [SerializeField] private GameObject objectToActivate;
        [SerializeField] private GameObject objectToDeactivate;
        [SerializeField] private ParticleSystem inactiveStateEffect;
        [SerializeField] private ParticleSystem activeStateEffect;

        private float timeToActivate;
        private float timeToDeactivate;

        private void Activate()
        {
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(true);
            }

            if (objectToDeactivate != null)
            {
                objectToDeactivate.SetActive(false);
            }

            if (activeStateEffect != null)
            {
                activeStateEffect.Play();
            }

            if (workMode == TrapWorkMode.Temporary 
                || workMode == TrapWorkMode.Periodical)
            {
                timeToDeactivate = lifeTime;
            }
        }

        private void Deactivate()
        {
            if (objectToActivate != null)
            {
                objectToActivate.SetActive(false);
            }

            if (objectToDeactivate != null)
            {
                objectToDeactivate.SetActive(true);
            }

            if (inactiveStateEffect != null)
            {
                inactiveStateEffect.Play();
            }

            if (workMode == TrapWorkMode.Periodical)
            {
                timeToActivate = activationPeriod;
            }
        }

        private void Start()
        {
            if (activationType == TrapActivationType.ActivateAtStart)
            {
                Activate();
            }
        }

        private void Update()
        {
            if (timeToDeactivate > 0f)
            {
                timeToDeactivate -= Time.deltaTime;

                if (timeToDeactivate <= 0f)
                {
                    Deactivate();
                }
            }

            if (timeToActivate > 0f)
            {
                timeToActivate -= Time.deltaTime;

                if (timeToActivate <= 0f)
                {
                    Activate();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (activationType == TrapActivationType.ActivateByTrigger 
                && other.TryGetComponent<PlayerController>(out _))
            {
                Activate();
            }
        }
    }

    [Serializable]
    public enum TrapActivationType
    {
        ActivateAtStart = 0,
        ActivateByTrigger = 1
    }

    [Serializable]
    public enum TrapWorkMode
    {
        Endless = 0,
        Periodical = 1,
        Temporary = 2
    }
}