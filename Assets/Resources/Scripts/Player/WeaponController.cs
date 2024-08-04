using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Heroicsolo.DI;
using Heroicsolo.Logics;
using Heroicsolo.Scripts.Inventory;
using Heroicsolo.Scripts.UI;
using Heroicsolo.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heroicsolo.Scripts.Player
{
    public class WeaponController : MonoBehaviour
    {
        private const float FirstShootDelay = 0.01f;

        [SerializeField] private ItemId weaponId;
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private PooledParticleSystem hitExplosion;
        [SerializeField][Min(0f)] private float shootPeriod = 1f;
        [SerializeField][Min(0f)] private float shootDistance = 20f;
        [SerializeField][Min(0f)] private float reloadTime = 3f;
        [SerializeField][Min(0)] private int ammo = 6;
        [SerializeField][Min(0f)] private float hitChance = 0.9f;

        [Inject] private IGameUIController gameUIController;
        [Inject] private IShootHelper shootHelper;

        private ItemParams itemParams;
        private bool isShooting = false;
        private bool isReloading = false;
        private float delayToShoot;
        private int currentAmmo;
        private Vector3 targetPoint;

        public ItemId WeaponID => weaponId;
        public bool IsReloading => isReloading;

        public void SetShooting(bool shooting, Vector3 targetPoint)
        {
            if (!isShooting && shooting)
            {
                isShooting = true;

                if (!isReloading)
                {
                    delayToShoot = FirstShootDelay;
                }
            }
            else if (!shooting)
            {
                isShooting = false;
            }

            this.targetPoint = targetPoint;
        }

        private float GetHitChanceByDist(float dist)
        {
            return Mathf.Lerp(1f, hitChance, dist / shootDistance);
        }

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);

            itemParams = ItemsCollection.ItemsParams[weaponId];
            currentAmmo = ammo;
        }

        private void OnEnable()
        {
            SystemsManager.InjectSystemsTo(this);

            currentAmmo = ammo;
            gameUIController.SetAmmo(ammo);
        }

        private void Shoot()
        {
            currentAmmo--;

            gameUIController.RemoveAmmo();

            if (currentAmmo == 0)
            {
                delayToShoot = reloadTime;
                isReloading = true;
            }
            else
            {
                delayToShoot = shootPeriod;
                isReloading = false;
            }

            muzzleFlash.Play();

            shootHelper.TryShoot(muzzleFlash.transform.position, 
                targetPoint - muzzleFlash.transform.position, 
                shootDistance, hitExplosion, itemParams, TeamType.Player, 
                GetHitChanceByDist((targetPoint - muzzleFlash.transform.position).magnitude));
        }

        private void Update()
        {
            if (delayToShoot > 0f)
            {
                delayToShoot -= Time.deltaTime;

                if (delayToShoot <= 0f)
                {
                    if (currentAmmo == 0)
                    {
                        isReloading = false;
                        currentAmmo = ammo;
                        gameUIController.SetAmmo(ammo);
                    }

                    if (isShooting)
                    {
                        Shoot();
                    }
                }
            }
        }
    }
}