using Heroicsolo.Heroicsolo.Player;
using Heroicsolo.Logics.Mobs;
using Heroicsolo.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Logics.Mobs
{
    [CreateAssetMenu(fileName = "New MobSpecialAttack", menuName = "Mob Special Attack", order = 53)]
    public class MobSpecialAttack : ScriptableObject
    {
        public AnimatorOverrideController AnimatorOverrideController;
        public AudioClip HitSound;
        [SerializeField] private PooledParticleSystem hitEffect;
        [SerializeField] private float hitEffectYOffset = 0f;
        [Min(0f)] public float Radius;
        [Min(0f)] public float Damage;
        public DamageType DamageType;
        [Range(0f, 1f)] public float NeededMobHealthPercent = 1f;
        [SerializeField] private bool shakeCamera;
        [SerializeField] private float cameraShakeTime = 1f;
        [SerializeField] private float cameraShakeStrength = 2f;

        private ITeamsManager teamsManager;
        private IGameController gameController;

        public void PerformAttack(Vector3 hitPosition, ITeamsManager teamsManager, IGameController gameController)
        {
            this.teamsManager ??= teamsManager;
            this.gameController ??= gameController;

            if (hitEffect != null)
            {
                PoolSystem.GetInstanceAtPosition(hitEffect, hitEffect.GetName(), hitPosition + Vector3.up * hitEffectYOffset);
            }

            teamsManager.GetEnemiesInRadius(hitPosition, TeamType.Enemies, Radius)
                .ForEach(x => x.GetDamage(Damage, DamageType));

            if (shakeCamera)
            {
                gameController.ShakeCamera(cameraShakeStrength, cameraShakeTime);
            }
        }
    }
}