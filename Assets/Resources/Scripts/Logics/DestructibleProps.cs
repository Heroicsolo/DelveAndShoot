using Heroicsolo.DI;
using Heroicsolo.Inventory;
using Heroicsolo.Logics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Heroicsolo.Scripts.Logics
{
    public class DestructibleProps : MonoBehaviour, IHittable
    {
        [SerializeField] private ParticleSystem destructionEffect;
        [SerializeField] [Min(0f)] private float durability = 1f;
        [SerializeField] private List<DamageType> damageResists = new List<DamageType>();
        [SerializeField] private string lootId;

        [Inject] private ILootManager lootManager;

        private float currDurability;

        public void Activate()
        {
            gameObject.SetActive(true);
            currDurability = durability;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void Die()
        {
            currDurability = 0f;

            if (destructionEffect != null)
            {
                destructionEffect.transform.parent = null;
                destructionEffect.Play();
            }

            if (!string.IsNullOrEmpty(lootId))
            {
                lootManager.GenerateRandomDrop(lootId, transform.position);
            }

            Destroy(gameObject);
        }

        public void DodgeDamage()
        {

        }

        public void GetDamage(float damage, DamageType damageType = DamageType.Physical)
        {
            if (durability <= 0f)
            {
                return;
            }

            if (damageResists.Contains(damageType))
            {
                return;
            }

            currDurability = Mathf.Clamp(currDurability - damage, 0f, durability);

            if (currDurability <= 0f)
            {
                Die();
            }
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public HittableType GetHittableType()
        {
            return HittableType.Props;
        }

        public TeamType GetTeamType()
        {
            return TeamType.Neutral;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public void Heal(float amount)
        {
            currDurability = Mathf.Clamp(currDurability + amount, 0f, durability);
        }

        public bool IsDead()
        {
            return currDurability <= 0f;
        }

        public void SetTeam(TeamType team)
        {
        }

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);
        }
    }
}