using Assets.FantasyInventory.Scripts.Data;
using Heroicsolo.DI;
using Heroicsolo.Utils;
using UnityEngine;
using VolumetricLines;

namespace Heroicsolo.Logics
{
    public interface IShootHelper : ISystem
    {
        LayerMask GetObstaclesMask();
        LayerMask GetTargetsMask();
        bool IsTargetReachable(Vector3 from, Vector3 targetPos);
        bool FindClosestTargetOnLine(Vector3 from, Vector3 direction, float maxDistance, TeamType shooterTeam, out IHittable targetHittable);
        bool FindClosestTargetInCone(Transform shooterTransform, Vector3 from, Vector3 direction, float maxDistance, float coneAngle, TeamType shooterTeam, out IHittable targetHittable);
        public bool TryShoot(Vector3 from, Vector3 direction, float maxDistance, PooledParticleSystem hitEffectPrefab, float damage, DamageType damageType, TeamType shooterTeam, float hitChance = 1f, VolumetricLineBehavior rayRenderer = null);
        public bool TryShoot(Vector3 from, Vector3 direction, float maxDistance, PooledParticleSystem hitEffectPrefab, ItemParams weaponParams, TeamType shooterTeam, float hitChance = 1f, VolumetricLineBehavior rayRenderer = null);
    }
}