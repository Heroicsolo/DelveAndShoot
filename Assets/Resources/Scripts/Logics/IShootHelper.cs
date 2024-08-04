using Assets.FantasyInventory.Scripts.Data;
using Heroicsolo.DI;
using Heroicsolo.Utils;
using UnityEngine;

namespace Heroicsolo.Logics
{
    public interface IShootHelper : ISystem
    {
        LayerMask GetObstaclesMask();
        LayerMask GetTargetsMask();
        bool IsTargetReachable(Vector3 from, Vector3 targetPos);
        bool FindClosestTargetOnLine(Vector3 from, Vector3 direction, float maxDistance, TeamType shooterTeam, out IHittable targetHittable);
        public bool TryShoot(Vector3 from, Vector3 direction, float maxDistance, PooledParticleSystem hitEffectPrefab, float damage, DamageType damageType, TeamType shooterTeam, float hitChance = 1f);
        public bool TryShoot(Vector3 from, Vector3 direction, float maxDistance, PooledParticleSystem hitEffectPrefab, ItemParams weaponParams, TeamType shooterTeam, float hitChance = 1f);
    }
}