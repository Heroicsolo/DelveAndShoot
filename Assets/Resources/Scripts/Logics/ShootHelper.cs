using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.DI;
using Heroicsolo.Utils;
using UnityEngine;
using VolumetricLines;

namespace Heroicsolo.Logics
{
    public class ShootHelper : MonoBehaviour, IShootHelper
    {
        [SerializeField] private LayerMask obstaclesMask;
        [SerializeField] private LayerMask targetsMask;

        [Inject] private ITeamsManager teamsManager;

        private RaycastHit[] hits;

        public LayerMask GetObstaclesMask()
        {
            return obstaclesMask;
        }

        public LayerMask GetTargetsMask()
        {
            return targetsMask;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public bool FindClosestTargetOnLine(Vector3 from, Vector3 direction, float maxDistance, TeamType shooterTeam, out IHittable targetHittable)
        {
            int hitCount = Physics.RaycastNonAlloc(from, direction.normalized, hits, maxDistance, targetsMask.value, QueryTriggerInteraction.Ignore);

            if (hitCount == 0)
            {
                targetHittable = null;
                return false;
            }
            else
            {
                for (int i = 0; i < hitCount; i++)
                {
                    if (hits[i].transform.TryGetComponent<IHittable>(out var hittable) && teamsManager.IsOppositeTeam(hittable, shooterTeam))
                    {
                        targetHittable = hittable;
                        return true;
                    }
                }

                targetHittable = null;
                return false;
            }
        }

        public bool IsTargetReachable(Vector3 from, Vector3 targetPos)
        {
            Vector3 direction = targetPos - from;

            int hitCount = Physics.RaycastNonAlloc(from, direction.normalized, hits, direction.magnitude, obstaclesMask.value, QueryTriggerInteraction.Ignore);

            return hitCount == 0;
        }

        public bool TryShoot(Vector3 from, Vector3 direction, float maxDistance, PooledParticleSystem hitEffectPrefab, float damage, DamageType damageType, TeamType shooterTeam, float hitChance = 1f, VolumetricLineBehavior rayRenderer = null)
        {
            int hitCount = Physics.RaycastNonAlloc(from, direction.normalized, hits, maxDistance, targetsMask.value, QueryTriggerInteraction.Ignore);

            if (hitCount > 0)
            {
                if (hits[0].transform.TryGetComponent<IHittable>(out var hittable) && teamsManager.IsOppositeTeam(hittable, shooterTeam))
                {
                    if (IsTargetReachable(from, hits[0].point))
                    {
                        if (Random.value <= hitChance)
                        {
                            hittable.GetDamage(damage, damageType);

                            PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);

                            ShowVolumetricRay(rayRenderer, from, hits[0].point);

                            return true;
                        }
                        else
                        {
                            hittable.DodgeDamage();

                            ShowVolumetricRay(rayRenderer, from, from + direction.normalized * maxDistance);

                            return false;
                        }
                    }
                    else
                    {
                        PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);

                        ShowVolumetricRay(rayRenderer, from, from + direction.normalized * maxDistance);

                        return false;
                    }
                }

                PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);

                ShowVolumetricRay(rayRenderer, from, hits[0].point);

                return false;
            }
            else if (Physics.RaycastNonAlloc(from, direction.normalized, hits, maxDistance, obstaclesMask.value, QueryTriggerInteraction.Ignore) > 0)
            {
                PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);

                ShowVolumetricRay(rayRenderer, from, hits[0].point);

                return false;
            }

            return false;
        }

        public bool TryShoot(Vector3 from, Vector3 direction, float maxDistance, PooledParticleSystem hitEffectPrefab, ItemParams weaponParams, TeamType shooterTeam, float hitChance = 1f, VolumetricLineBehavior rayRenderer = null)
        {
            int hitCount = Physics.RaycastNonAlloc(from, direction.normalized, hits, maxDistance, targetsMask.value, QueryTriggerInteraction.Ignore);

            if (hitCount > 0)
            {
                if (hits[0].transform.TryGetComponent<IHittable>(out var hittable) && teamsManager.IsOppositeTeam(hittable, shooterTeam))
                {
                    if (IsTargetReachable(from, hits[0].point))
                    {
                        if (Random.value > hitChance)
                        {
                            hittable.DodgeDamage();
                            return false;
                        }

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

                        PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);

                        ShowVolumetricRay(rayRenderer, from, hits[0].point);

                        return true;
                    }
                    else
                    {
                        PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);

                        ShowVolumetricRay(rayRenderer, from, from + direction.normalized * maxDistance);

                        return false;
                    }
                }

                PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);

                ShowVolumetricRay(rayRenderer, from, from + direction.normalized * maxDistance);
            }
            else if (Physics.RaycastNonAlloc(from, direction.normalized, hits, maxDistance, obstaclesMask.value, QueryTriggerInteraction.Ignore) > 0)
            {
                PoolSystem.GetInstanceAtPosition(hitEffectPrefab, hitEffectPrefab.GetName(), hits[0].point);

                ShowVolumetricRay(rayRenderer, from, from + direction.normalized * maxDistance);

                return false;
            }

            return false;
        }

        private void ShowVolumetricRay(VolumetricLineBehavior volumetricLine, Vector3 startPos, Vector3 targetPos)
        {
            if (volumetricLine != null)
            {
                volumetricLine.gameObject.SetActive(true);
                volumetricLine.SetStartAndEndPoints(volumetricLine.transform.InverseTransformPoint(startPos), 
                    volumetricLine.transform.InverseTransformPoint(targetPos));
            }
        }

        private void Start()
        {
            hits = new RaycastHit[3];
        }
    }
}