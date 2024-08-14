using Assets.Resources.Scripts.Logics;
using Heroicsolo.Logics.Props;
using System;
using System.Collections;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public class Ignatable : HittableProp, IInteractable
    {
        [SerializeField] GameObject fire;
        [SerializeField] float minDamageToIgnite = 1;
        [SerializeField] float minDamageToExtinguish = 1;
        [SerializeField] bool isInteractable = true;
        [SerializeField] bool isIgnited = false;

        public bool IsIgnited
        {
            get => isIgnited;
            set
            {
                if (isIgnited && !value)
                    Extinguish();
                else if (!isIgnited && value)
                    Ignite();
            }
        }

        public bool IsInteractable => isInteractable;

        public override void Die()
        {
        }

        public override void GetDamage(float damage, DamageType damageType = DamageType.Physical)
        {
            switch (damageType)
            {
                case DamageType.Fire:
                    if (damage > minDamageToIgnite)
                        Ignite();
                    break;
                case DamageType.Frost:
                    if (damage > minDamageToExtinguish)
                        Extinguish();
                    break;
                default:
                    break;
            }
        }

        private void Extinguish()
        {
            isIgnited = false;
            fire.SetActive(false);
        }

        private void Ignite()
        {
            isIgnited = true;
            fire.SetActive(true);
        }

        public override void Heal(float amount)
        {
        }

        public override bool IsDead()
        {
            return false;
        }

        public void Interact()
        {
            if (isInteractable)
                IsIgnited = !IsIgnited;
        }

        IInteractable.InteractionType IInteractable.GetInteractionType()
        {
            return IInteractable.InteractionType.Torch;
        }
    }
}