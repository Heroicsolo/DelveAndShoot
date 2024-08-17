using Heroicsolo.Heroicsolo.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Logics.Traps
{
    public class DamageArea : MonoBehaviour
    {
        [SerializeField] private DamageType damageType;
        [SerializeField] [Min(0f)] private float tickPeriod = 1f;
        [SerializeField] [Min(0f)] private float damagePerTickMin = 0f;
        [SerializeField] [Min(0f)] private float damagePerTickMax = 0f;

        private List<IHittable> hittables = new List<IHittable>();

        private void OnTriggerStay(Collider other)
        {
            if (other.TryGetComponent<IHittable>(out var hittable))
            {
                if (!hittables.Contains(hittable))
                {
                    hittables.Add(hittable);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IHittable>(out var hittable))
            {
                if (hittables.Contains(hittable))
                {
                    hittables.Remove(hittable);
                }
            }
        }

        private IEnumerator DamageDealer()
        {
            while (true)
            {
                hittables.ForEach(x => x.GetDamage(Random.Range(damagePerTickMin, damagePerTickMax), damageType));

                yield return new WaitForSeconds(tickPeriod);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(DamageDealer());
        }

        private void OnDisable()
        {
            hittables.Clear();
            StopAllCoroutines();
        }
    }
}