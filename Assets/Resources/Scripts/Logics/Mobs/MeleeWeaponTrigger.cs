using Heroicsolo.Heroicsolo.Player;
using UnityEngine;

namespace Heroicsolo.Logics.Mobs
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