using System.Collections;
using UnityEngine;

namespace Heroicsolo.Logics.Props
{
    public abstract class HittableProp : MonoBehaviour, IHittable
    {
        public abstract void Die();
        public abstract void Heal(float amount);
        public abstract bool IsDead();
        public abstract void GetDamage(float damage, DamageType damageType = DamageType.Physical);
        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void DodgeDamage()
        {
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Transform GetHitPivot()
        {
            return transform;
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

        public void SetTeam(TeamType team)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Cant change team of the prop {gameObject.name}");
#endif
        }
    }
}