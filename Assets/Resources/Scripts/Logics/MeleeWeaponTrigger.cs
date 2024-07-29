using Heroicsolo.Logics;
using Heroicsolo.Scripts.Logics;
using Heroicsolo.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Resources.Scripts.Logics
{
    public class MeleeWeaponTrigger : MonoBehaviour
    {
        [SerializeField] private DamageType damageType = DamageType.Physical;

        private GenericMob ownerMob;

        private void Start()
        {
            ownerMob = GetComponentInParent<GenericMob>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var pc) && ownerMob.IsAttacking())
            {
                pc.GetDamage(ownerMob.AttackDamage, damageType);
            }
        }
    }
}