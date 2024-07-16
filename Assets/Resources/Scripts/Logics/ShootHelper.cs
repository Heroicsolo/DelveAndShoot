using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.DI;
using Heroicsolo.Utils;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public class ShootHelper : MonoBehaviour, IShootHelper
    {
        [SerializeField] private LayerMask obstaclesMask;

        [Inject] private ITeamsManager teamsManager;

        private RaycastHit[] hits;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool TryShoot(Vector3 from, Vector3 direction, float maxDistance, PooledParticleSystem hitEffectPrefab, float damage, DamageType damageType, TeamType shooterTeam)
        {
            if (Physics.RaycastNonAlloc(from, direction, hits, maxDistance, obstaclesMask.value, QueryTriggerInteraction.Ignore) > 0)
            {
                if (hits[0].transform.TryGetComponent<IHittable>(out var hittable) && teamsManager.IsOppositeTeam(hittable, shooterTeam))
                {
                    hittable.GetDamage(damage, damageType);
                    return true;
                }

                PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);
            }

            return false;
        }

        public bool TryShoot(Vector3 from, Vector3 direction, float maxDistance, PooledParticleSystem hitEffectPrefab, ItemParams weaponParams, TeamType shooterTeam)
        {
            if (Physics.RaycastNonAlloc(from, direction, hits, maxDistance, obstaclesMask.value, QueryTriggerInteraction.Ignore) > 0)
            {
                Debug.DrawLine(from, hits[0].point, Color.red, 1f);

                if (hits[0].transform.TryGetComponent<IHittable>(out var hittable) && teamsManager.IsOppositeTeam(hittable, shooterTeam))
                {
                    if (weaponParams.TryGetProperty(PropertyId.PhysicDamage, out var physDmg))
                    {
                        hittable.GetDamage(physDmg.Value, DamageType.Physical);
                    }
                    if (weaponParams.TryGetProperty(PropertyId.MagicDamage, out var magDmg))
                    {
                        hittable.GetDamage(magDmg.Value, DamageType.Curse);
                    }
                    if (weaponParams.TryGetProperty(PropertyId.FrostDamage, out var frstDmg))
                    {
                        hittable.GetDamage(frstDmg.Value, DamageType.Frost);
                    }
                    if (weaponParams.TryGetProperty(PropertyId.FireDamage, out var fireDmg))
                    {
                        hittable.GetDamage(fireDmg.Value, DamageType.Fire);
                    }
                    if (weaponParams.TryGetProperty(PropertyId.EnergyDamage, out var enrgDmg))
                    {
                        hittable.GetDamage(enrgDmg.Value, DamageType.Electrical);
                    }
                    return true;
                }

                PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);
            }

            return false;
        }

        private void Start()
        {
            hits = new RaycastHit[3];
        }
    }
}