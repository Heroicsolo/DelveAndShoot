using Assets.Resources.Scripts.Logics;
using Heroicsolo.DI;
using Heroicsolo.Logics.Props;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Heroicsolo.Logics.ActionManager;

namespace Heroicsolo.Logics
{
    public class Ignatable : ManagedActor, IInteractable //TODO: Add IsExtinguishable
    {
        [SerializeField] GameObject fire;
        [SerializeField] float minDamageToIgnite = 1;
        [SerializeField] float minDamageToExtinguish = 1;
        [SerializeField] bool isInteractable = true;
        [SerializeField] bool isIgnited = false;
        [Inject] private IActionManager actionManager;

        public bool IsIgnited
        {
            get => isIgnited;
            set
            {
                if (isIgnited && !value)
                    Do("Extinguish");
                else if (!isIgnited && value)
                    Do("Ignite");
            }
        }

        public bool IsInteractable => isInteractable;

        internal override IActionManager ActionManager => actionManager;

        public new void GetDamage(float damage, DamageType damageType = DamageType.Physical)
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

        [ActorAction]
        private bool Extinguish(Dictionary<string, object> bag = null)
        {
            isIgnited = false;
            fire.SetActive(false);
            return true;
        }

        [ActorAction]
        private bool Ignite(Dictionary<string, object> bag = null)
        {
            isIgnited = true;
            fire.SetActive(true);
            return true;
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

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);
            actionManager.RegisterManagedActor(typeof(Ignatable));
        }
    }
}