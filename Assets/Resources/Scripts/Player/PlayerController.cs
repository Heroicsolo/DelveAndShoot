using Assets.FantasyInventory.Scripts.Enums;
using DG.Tweening;
using Heroicsolo.DI;
using Heroicsolo.Inventory;
using Heroicsolo.Logics;
using Heroicsolo.Logics;
using Heroicsolo.Scripts.UI;
using Heroicsolo.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Heroicsolo.Heroicsolo.Player
{
    public class PlayerController : MonoBehaviour, ICharacter
    {
        private readonly int JumpAnimHash = Animator.StringToHash("Jump");
        private readonly int ShootingAnimHash = Animator.StringToHash("Shooting");
        private readonly int DieAnimHash = Animator.StringToHash("Die");
        private const int AimRotateIterations = 10;
        private const float AimRotateAngleLimit = 90f;
        private const float AimRotateDistanceLimit = 1.5f;

        [Header("Movement Params")]
        [SerializeField] private float runSpeedFatiguedCoef = 0.6f;
        [SerializeField] private float turnSpeed = 120f;
        [SerializeField] private float cameraMoveSpeed = 10f;
        [SerializeField] private float directionChangeSpeed = 10f;
        [SerializeField] private float jumpPower = 2f;
        [SerializeField][Range(0f, 1f)] private float fatigueEndThreshold = 0.7f;
        [SerializeField][Min(0f)] private float staminaSpendingForRun = 2f;
        [SerializeField] private LayerMask floorMask;

        [Header("Weapons")]
        [SerializeField] private List<WeaponController> weapons = new List<WeaponController>();

        [Header("Character Stats")]
        [SerializeField] private List<CharacterStat> stats = new List<CharacterStat>();

        [Header("Other")]
        [SerializeField] private HidingObjectsManager hidingObjectsManager;
        [SerializeField] private FloatingText combatTextPrefab;
        [SerializeField] private Transform playerCanvasTransform;
        [SerializeField] private List<AimingBoneInfo> aimingBones = new List<AimingBoneInfo>();

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource foostepsAudioSource;
        [SerializeField] private AudioClip fallSound;

        [Inject] private IGameUIController gameUIController;
        [Inject] private IPlayerProgressionManager playerProgressionManager;
        [Inject] private ICharacterStatsManager characterStatsManager;
        [Inject] private IShootHelper shootHelper;
        [Inject] private IInventoryManager inventoryManager;

        private CharacterController characterController;
        private Animator animator;
        private Vector3 runDirection;
        private Vector3 targetRunDirection;
        private Vector3 camOffset;
        private Transform cameraTransform;
        private float jumpSpeed;
        private bool isFatigued;
        private bool isJumping;
        private Vector3 lookPos;
        private Vector3 shootPos;
        private Dictionary<CharacterStatType, CharacterStat> statsDict = new Dictionary<CharacterStatType, CharacterStat>();
        private RaycastHit[] raycastHits;
        private WeaponController weaponController;
        private Dictionary<Transform, float> aimingBonesTransforms = new Dictionary<Transform, float>();

        public List<CharacterStat> GetCharacterStats()
        {
            return stats;
        }

        public void SubscribeToDamageGot(Action<float> onDamageGot)
        {
        }

        public void SubscribeToDamageDodged(Action onDamageDodged)
        {
        }

        public CharacterStat GetCharacterStat(CharacterStatType characterStatType)
        {
            return stats.Find(s => s.StatType == characterStatType);
        }

        public void Heal(float amount)
        {
            statsDict[CharacterStatType.Health].Change(amount);
        }

        public void DodgeDamage()
        {

        }

        public void GetDamage(float damage, DamageType damageType = DamageType.Physical)
        {
            if (statsDict.ContainsKey(CharacterStatType.Armor) && damageType == DamageType.Physical)
            {
                damage *= (1f - characterStatsManager.GetDamageAbsorbPercentage(statsDict[CharacterStatType.Armor].Value));
            }

            if (damage > 0f)
            {
                FloatingText ft = PoolSystem.GetInstanceAtPosition(combatTextPrefab, combatTextPrefab.GetName(), playerCanvasTransform.position, playerCanvasTransform);
                ft.SetText($"-{Mathf.CeilToInt(damage)}");
                ft.SetCurved(true);
            }

            statsDict[CharacterStatType.Health].Change(-damage);
        }

        public void PlaySound(AudioClip clip)
        {
            audioSource.PlayOneShot(clip);
        }

        public void Die()
        {
            statsDict[CharacterStatType.Health].Set(0);
        }

        public bool IsDead()
        {
            return statsDict[CharacterStatType.Health].Value == 0;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public Transform GetHitPivot()
        {
            return transform;
        }

        public HittableType GetHittableType()
        {
            return HittableType.Humanoid;
        }

        public void SetTeam(TeamType team)
        {
        }

        public TeamType GetTeamType()
        {
            return TeamType.Player;
        }

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }

        public void EquipWeapon(ItemId weaponId)
        {
            weapons.ForEach(weapon =>
            {
                weapon.gameObject.SetActive(weapon.WeaponID == weaponId);
                weaponController = weapon;
            });
        }

        private void OnLevelUp(int level)
        {
            statsDict.Values.ToList().ForEach(s => s.ScaleByLevel(level));
        }

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);

            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            cameraTransform = Camera.main.transform;
            camOffset = cameraTransform.localPosition;
            cameraTransform.parent = null;
            cameraTransform.rotation = Quaternion.Euler(45f, 0f, 0f);
            hidingObjectsManager.SetPlayerTransform(transform);

            EquipWeapon(inventoryManager.GetEquippedItems().Single(x => x.Params.Type == ItemType.Weapon).Id);

            raycastHits = new RaycastHit[2];

            aimingBones.ForEach(b => aimingBonesTransforms.Add(animator.GetBoneTransform(b.BoneType), b.Weight));

            InitState();
        }

        private void InitState()
        {
            statsDict.Clear();

            foreach (CharacterStat characterStat in stats)
            {
                statsDict.Add(characterStat.StatType, characterStat);
            }

            statsDict[CharacterStatType.MoveSpeed].Init((_, _, _) => { });
            statsDict[CharacterStatType.Health].Init(gameUIController.OnHealthChanged);
            statsDict[CharacterStatType.Health].SetRegenState(true);
            statsDict[CharacterStatType.Stamina].Init(gameUIController.OnStaminaChanged);

            playerProgressionManager.SubscribeToLevelUpEvent(OnLevelUp);
        }

        private void Jump()
        {
            animator.SetTrigger(JumpAnimHash);
            jumpSpeed = jumpPower;
            isJumping = true;
        }

        private void ProccessAim()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (shootHelper.FindClosestTargetOnLine(ray.origin, ray.direction, 100f, TeamType.Player, out var hittable))
            {
                gameUIController.SetCursorState(CursorState.Targeted);
                shootPos = hittable.GetHitPivot().position;
            }
            else if (Physics.RaycastNonAlloc(ray, raycastHits, 100f, shootHelper.GetObstaclesMask().value) > 0)
            {
                gameUIController.SetCursorState(CursorState.Aim);
                shootPos = raycastHits[0].point;
            }
            else
            {
                gameUIController.SetCursorState(CursorState.Default);
                shootPos = transform.position + transform.forward * 10f;
            }

            lookPos = shootPos;
            lookPos.y = transform.position.y;
            transform.LookAt(lookPos);
            RotateBones();
        }

        private Vector3 GetCorrectedTargetPosition(Vector3 targetPos)
        {
            Vector3 targetDirection = targetPos - weaponController.MuzzleTransform.position;
            Vector3 aimDirection = weaponController.MuzzleTransform.forward;
            float targetDistance = targetDirection.magnitude;
            float blendOut = 0f;

            float targetAngle = Vector3.Angle(targetDirection, aimDirection);

            if (targetAngle > AimRotateAngleLimit)
            {
                blendOut += (targetAngle - AimRotateAngleLimit) / 50f;
            }

            if (targetDistance < AimRotateDistanceLimit)
            {
                blendOut += AimRotateDistanceLimit - targetDistance;
            }

            Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);

            return weaponController.MuzzleTransform.position + direction;
        }

        private void RotateBones()
        {
            Vector3 targetPos = GetCorrectedTargetPosition(shootPos);

            for (int i = 0; i < AimRotateIterations; i++)
            {
                foreach (var bone in aimingBonesTransforms)
                {
                    AimBoneAtTarget(bone.Key, targetPos, bone.Value);
                }
            }
        }

        private void AimBoneAtTarget(Transform bone, Vector3 targetPos, float weight)
        {
            Vector3 aimDirection = weaponController.MuzzleTransform.forward;
            Vector3 targetDirection = targetPos - weaponController.MuzzleTransform.position;
            Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
            Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);

            bone.rotation = blendedRotation * bone.rotation;
        }

        private void ProccessInputs()
        {
            targetRunDirection = Vector3.zero;

            if (Input.GetKey(KeyCode.W))
            {
                targetRunDirection.z += 1f;
            }
            if (Input.GetKey(KeyCode.S))
            {
                targetRunDirection.z -= 1f;
            }
            if (Input.GetKey(KeyCode.A))
            {
                targetRunDirection.x -= 1f;
            }
            if (Input.GetKey(KeyCode.D))
            {
                targetRunDirection.x += 1f;
            }

            if (Input.GetKeyDown(KeyCode.Space) && characterController.isGrounded)
            {
                Jump();
            }

            if (Input.GetMouseButton(0))
            {
                SetShooting(true);
            }
            else
            {
                SetShooting(false);
            }
        }

        private void ProccessMovement()
        {
            runDirection = Vector3.Lerp(runDirection, targetRunDirection, directionChangeSpeed * Time.deltaTime);

            Vector3 localDirection = transform.InverseTransformDirection(runDirection);

            animator.SetFloat("Forward", localDirection.z, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", localDirection.x, 0.1f, Time.deltaTime);

            if (targetRunDirection.magnitude > 0f)
            {
                statsDict[CharacterStatType.Stamina].Change(-staminaSpendingForRun * Time.deltaTime);
                statsDict[CharacterStatType.Stamina].SetRegenState(false);

                if (!foostepsAudioSource.isPlaying)
                {
                    foostepsAudioSource.Play();
                }

                foostepsAudioSource.DOFade(1f, 0.2f);
            }
            else
            {
                statsDict[CharacterStatType.Stamina].SetRegenState(true);

                foostepsAudioSource.DOFade(0f, 0.5f);
            }

            if (!characterController.isGrounded)
            {
                jumpSpeed -= 9.8f * Time.deltaTime;
            }
            else if (isJumping)
            {
                isJumping = false;
                audioSource.PlayOneShot(fallSound);
            }

            characterController.Move((runDirection *
                (isFatigued ? runSpeedFatiguedCoef * statsDict[CharacterStatType.MoveSpeed].Value : statsDict[CharacterStatType.MoveSpeed].Value)
                + Vector3.up * jumpSpeed) * Time.deltaTime);
        }

        private void ProccessStats()
        {
            if (statsDict[CharacterStatType.Stamina].Value == 0f)
            {
                isFatigued = true;
            }
            else
            {
                if (statsDict[CharacterStatType.Stamina].Percent >= fatigueEndThreshold)
                {
                    isFatigued = false;
                }
            }

            statsDict[CharacterStatType.Health].Update(Time.deltaTime);
            statsDict[CharacterStatType.Stamina].Update(Time.deltaTime);
        }

        private void SetShooting(bool shooting)
        {
            animator.SetBool(ShootingAnimHash, shooting && !weaponController.IsReloading);
            weaponController.SetShooting(shooting, shootPos);
        }

        private void Update()
        {
            ProccessInputs();
            ProccessMovement();
            ProccessStats();
        }

        private void LateUpdate()
        {
            ProccessAim();

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, transform.position + camOffset, cameraMoveSpeed * Time.deltaTime);
        }
    }

    [Serializable]
    public class AimingBoneInfo
    {
        public HumanBodyBones BoneType;
        public float Weight = 1f;
    }
}