using Heroicsolo.DI;
using Heroicsolo.Inventory;
using Heroicsolo.Heroicsolo.Player;
using Heroicsolo.Scripts.UI;
using Heroicsolo.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Animations;
using Unity.VisualScripting;

namespace Heroicsolo.Logics.Mobs
{
    internal class GenericMob : Mob
    {
        private readonly int WalkAnimHash = Animator.StringToHash("Walk");
        private readonly int AttackAnimHash = Animator.StringToHash("Attack");
        private readonly int DieAnimHash = Animator.StringToHash("Die");
        private const float TargetReachThreshold = 0.2f;

        [SerializeField] private string typeName;
        [SerializeField] private HittableType creatureType;
        [SerializeField] private TeamType defaultTeam;
        [SerializeField] private BasicMobStrategy defaultMobStrategy;

        [Header("Mob Balance")]
        [SerializeField] private List<CharacterStat> stats = new();
        [SerializeField][Min(0f)] private float attackDistance = 3f;
        [SerializeField][Min(0f)] private float dissapearTime = 5f;
        [SerializeField][Min(0f)] private float attackPowerMin = 1f;
        [SerializeField][Min(0f)] private float attackPowerMax = 1f;

        [SerializeField] private List<MobSpecialAttack> specialAttacks = new();
        [SerializeField] [Range(0f, 1f)] private float specialAttackChance = 0.25f;

        [Header("Patrol Points")]
        [SerializeField] private List<Transform> patrolPoints = new();

        [Header("Mob Rewards")]
        [SerializeField] [Min(0)] private int expReward = 100;
        [SerializeField] private string lootId;

        [Header("Visuals")]
        [SerializeField] private MobCanvas mobCanvas;
        [SerializeField] private Transform mobCircle;
        [SerializeField] private Transform specialAttackCircle;
        [SerializeField] private Transform hitPivot;

        [Header("Dialogs")]
        [SerializeField] private Sprite dialogAvatar;
        [SerializeField] private List<string> aggroReplics = new();
        [SerializeField] private List<string> deathReplics = new();

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioSource footstepsAudioSource;
        [SerializeField] private AudioClip aggroSound;
        [SerializeField] private List<AudioClip> attackSounds;
        [SerializeField] private List<AudioClip> specialAttackSounds;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip footstepsSound;

        [Inject] private ICharacterStatsManager characterStatsManager;
        [Inject] private IPlayerProgressionManager playerProgressionManager;
        [Inject] private ILootManager lootManager;
        [Inject] private ITeamsManager teamsManager;
        [Inject] private IDialogPopup dialogPopup;

        #region Private Fields
        private Dictionary<CharacterStatType, CharacterStat> statsDict = new Dictionary<CharacterStatType, CharacterStat>();

        private NavMeshAgent agent;
        private Animator animator;
        private BotState botState;
        private PlayerController playerController;
        private Transform lastPatrolPoint;
        private TeamType currentTeam;
        private Vector3 spawnPoint;
        private bool weaponActive;
        private Action<float> OnDamageGot;
        private Action OnDamageDodged;
        private bool aggroDialogPlayed;
        private IMobStrategy mobStrategyInstance;
        private RuntimeAnimatorController defaultAnimatorController;
        private MobSpecialAttack currentSpecialAttack;
        #endregion

        #region Public Fields
        public float AttackDamage => UnityEngine.Random.Range(attackPowerMin, attackPowerMax);
        public float AttackDistance => attackDistance;
        
        public BotState BotState
        {
            get { return botState; }
            set { botState = value; }
        }
        public List<Transform> PatrolPoints => patrolPoints;
        public Vector3 SpawnPoint => spawnPoint;
        public Dictionary<CharacterStatType, CharacterStat> StatsDict => statsDict;
        #endregion

        public void SetStrategy(IMobStrategy strategy)
        {
            mobStrategyInstance = null;
            mobStrategyInstance = ScriptableObject.CreateInstance(strategy.GetType()) as IMobStrategy;
            mobStrategyInstance.Init(this, agent, playerController);
            mobStrategyInstance.SwitchState(botState);
        }

        #region Poolable Methods
        public override GameObject GetGameObject()
        {
            return gameObject;
        }

        public override string GetName()
        {
            return gameObject.name;
        }

        public override Transform GetTransform()
        {
            return transform;
        }

        public override Transform GetHitPivot()
        {
            if (hitPivot != null)
            {
                return hitPivot;
            }

            return transform;
        }

        public override void SetName(string name)
        {
            gameObject.name = name;
        }
        #endregion

        #region Mob Movement
        public void SelectNextPatrolPoint()
        {
            Transform nextPoint = lastPatrolPoint != null ? patrolPoints.GetRandomElementExceptOne(lastPatrolPoint) : patrolPoints.GetRandomElement();

            StartMovement(nextPoint.position);

            lastPatrolPoint = nextPoint;
        }
        public bool IsReachedNextPoint()
        {
            if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            {
                return false;
            }

            if (agent.remainingDistance < TargetReachThreshold)
            {
                return true;
            }

            return false;
        }
        public void StopMovement()
        {
            if (agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.isStopped = true;
            }

            footstepsAudioSource.Stop();

            animator.SetBool(WalkAnimHash, false);
        }
        public void StartMovement(Vector3 destination)
        {
            if (agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
                agent.SetDestination(destination);
                animator.SetBool(WalkAnimHash, true);
                animator.SetBool(AttackAnimHash, false);

                if (!footstepsAudioSource.isPlaying && footstepsAudioSource.clip != null)
                {
                    footstepsAudioSource.Play();
                }
            }
        }
        public void FollowPlayer()
        {
            mobStrategyInstance.SwitchState(BotState.FollowPlayer);
        }
        #endregion

        #region Mob State
        public void ResetHealth()
        {
            statsDict[CharacterStatType.Health].Reset();
        }
        public void ResetState()
        {
            if (patrolPoints.Count > 0)
            {
                mobStrategyInstance.SwitchState(BotState.Patrolling);
            }
            else
            {
                mobStrategyInstance.SwitchState(BotState.Idle);
            }
        }
        public override void Activate()
        {
            InitState();
        }
        public override void Deactivate()
        {
            mobStrategyInstance.SwitchState(BotState.Sleeping);
        }
        public override void Die()
        {
            mobStrategyInstance.SwitchState(BotState.Death);

            if (deathReplics.Count > 0)
            {
                dialogPopup.ShowMessage(deathReplics.GetRandomElement(), dialogAvatar);
            }

            if (deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }

            SetAnimatorState(BotAnimatorState.Death);
            StopMovement();
            HideMobCanvasAndCircle();
            SpawnLoot();
            Invoke(nameof(ReturnToPool), dissapearTime);
            playerProgressionManager.AddExperience(expReward);
        }
        public override bool IsDead()
        {
            return statsDict[CharacterStatType.Health].Value <= 0f;
        }
        #endregion

        #region Mob Events
        public override void SubscribeToDamageGot(Action<float> onDamageGot)
        {
            OnDamageGot += onDamageGot;
        }

        public override void SubscribeToDamageDodged(Action onDamageDodged)
        {
            OnDamageDodged += onDamageDodged;
        }
        #endregion

        #region Combat
        public void ActivateWeaponTrigger()
        {
            weaponActive = true;
        }
        public void DeactivateWeaponTrigger()
        {
            weaponActive = false;
        }
        public bool IsAttacking()
        {
            return botState == BotState.Attacking && weaponActive;
        }
        public bool IsPlayerOutranged()
        {
            if (mobStrategyInstance is StationaryMobStrategy &&
                Vector3.Distance(transform.position, playerController.transform.position) > attackDistance)
            {
                return true;
            }

            return false;
        }
        public bool IsDamageable()
        {
            return botState != BotState.Evade && botState != BotState.Death && statsDict[CharacterStatType.Health].Value > 0f;
        }
        public void OnAggro()
        {
            if (!aggroDialogPlayed && aggroReplics.Count > 0)
            {
                dialogPopup.ShowMessage(aggroReplics.GetRandomElement(), dialogAvatar);
                aggroDialogPlayed = true;
            }

            if (aggroSound != null)
            {
                audioSource.PlayOneShot(aggroSound);
            }
        }
        public override void DodgeDamage()
        {
            OnDamageDodged?.Invoke();
        }
        public override void GetDamage(float damage, DamageType damageType = DamageType.Physical)
        {
            if (botState != BotState.Evade)
            {
                DodgeDamage();
            }

            if (!IsDamageable() || damage <= 0f)
            {
                return;
            }

            if (statsDict.ContainsKey(CharacterStatType.Armor) && damageType == DamageType.Physical)
            {
                damage *= (1f - characterStatsManager.GetDamageAbsorbPercentage(statsDict[CharacterStatType.Armor].Value));
            }

            OnDamageGot?.Invoke(damage);

            statsDict[CharacterStatType.Health].Change(-damage);

            mobStrategyInstance.OnGetDamage(damage, damageType);
        }
        public override void Heal(float amount)
        {
            statsDict[CharacterStatType.Health].Change(amount);
        }
        public void OnMeleeAttackStarted()
        {
            if (attackSounds != null)
            {
                audioSource.PlayOneShot(attackSounds.GetRandomElement());
            }
        }
        public void OnMeleeAttackPerformed()
        {
            playerController.GetDamage(AttackDamage, DamageType.Physical);
        }
        public void OnSpecialAttackStarted()
        {
            if (specialAttackSounds != null)
            {
                audioSource.PlayOneShot(specialAttackSounds.GetRandomElement());
            }
        }
        public void OnSpecialAttackPerformed()
        {
            currentSpecialAttack.PerformAttack(transform.position, teamsManager);
            if (!IsPlayerOutranged())
            {
                currentSpecialAttack = null;
            }
            specialAttackCircle.gameObject.SetActive(false);
            animator.runtimeAnimatorController = defaultAnimatorController;
        }
        public override HittableType GetHittableType()
        {
            return creatureType;
        }
        public void SpawnLoot()
        {
            lootManager.GenerateRandomDrop(lootId, transform.position);
        }
        private void TrySelectSpecialAttack()
        {
            if (specialAttacks.Count > 0 && (IsPlayerOutranged() || UnityEngine.Random.value < specialAttackChance))
            {
                List<MobSpecialAttack> selectedAttacks = new List<MobSpecialAttack>();

                foreach (MobSpecialAttack specialAttack in specialAttacks)
                {
                    if (specialAttack.NeededMobHealthPercent >= statsDict[CharacterStatType.Health].Percent)
                    {
                        selectedAttacks.Add(specialAttack);
                    }
                }

                currentSpecialAttack = selectedAttacks.GetRandomElement();

                specialAttackCircle.gameObject.SetActive(true);
                specialAttackCircle.localScale = currentSpecialAttack.Radius * Vector3.one / transform.localScale.x;

                if (currentSpecialAttack.AnimatorOverrideController != null)
                {
                    animator.runtimeAnimatorController = currentSpecialAttack.AnimatorOverrideController;
                }
            }
        }
        #endregion

        #region Mob Stats
        public override List<CharacterStat> GetCharacterStats()
        {
            return stats;
        }
        public override CharacterStat GetCharacterStat(CharacterStatType characterStatType)
        {
            return stats.Find(s => s.StatType == characterStatType);
        }

        #endregion

        #region Mob Team
        public override TeamType GetTeamType()
        {
            return currentTeam;
        }
        public override void SetTeam(TeamType team)
        {
            currentTeam = team;
        }
        public GenericMob GetNearestTeamMember()
        {
            return (GenericMob)teamsManager.GetNearestTeamMember(currentTeam, this, true);
        }
        #endregion

        #region Mob Visuals
        public void SetAnimatorState(BotAnimatorState animatorState)
        {
            switch (animatorState)
            {
                case BotAnimatorState.Idle:
                    animator.SetBool(WalkAnimHash, false);
                    animator.SetBool(AttackAnimHash, false);
                    break;
                case BotAnimatorState.Walk:
                    animator.SetBool(WalkAnimHash, true);
                    animator.SetBool(AttackAnimHash, false);
                    break;
                case BotAnimatorState.Death:
                    animator.SetBool(WalkAnimHash, false);
                    animator.SetBool(AttackAnimHash, false);
                    animator.SetTrigger(DieAnimHash);
                    break;
                case BotAnimatorState.Attack:
                    animator.SetBool(WalkAnimHash, false);
                    animator.SetBool(AttackAnimHash, true);
                    break;
            }
        }
        public void HideMobCanvasAndCircle()
        {
            if (mobCanvas != null)
            {
                mobCanvas.gameObject.SetActive(false);
            }

            mobCircle.gameObject.SetActive(false);
        }
        #endregion

        #region Private Methods
        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);

#if UNITY_EDITOR
            if (string.IsNullOrEmpty(typeName))
            {
                Debug.LogError($"Mob prefab {gameObject.name} spawned without proper pool name assigned");
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
            weaponActive = false;

            if (footstepsSound != null)
            {
                footstepsAudioSource.clip = footstepsSound;
                footstepsAudioSource.loop = true;
            }
        }

        private void Update()
        {
            if (botState != BotState.Death)
            {
                mobStrategyInstance.UpdateCurrentState(Time.deltaTime);

                if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == AttackAnimHash 
                    && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    TrySelectSpecialAttack();
                }
            }
        }

        private void InitState()
        {
            SystemsManager.InjectSystemsTo(this);

            statsDict.Clear();

            foreach (CharacterStat characterStat in stats)
            {
                statsDict.Add(characterStat.StatType, characterStat);
            }

            statsDict[CharacterStatType.Health].SetRegenState(true);
            statsDict[CharacterStatType.Health].Reset();

            playerController = FindObjectOfType<PlayerController>();

            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            animator.ResetTrigger(DieAnimHash);

            defaultAnimatorController = animator.runtimeAnimatorController;

            if ((!agent.isOnNavMesh || !agent.isActiveAndEnabled) && !(defaultMobStrategy is StationaryMobStrategy))
            {
                //Mob is spawned on bad position, return it to pool
                ReturnToPool();
                return;
            }

            lastPatrolPoint = null;

            currentTeam = defaultTeam;

            teamsManager.RegisterTeamMember(this, currentTeam);

            spawnPoint = transform.position;

            weaponActive = false;

            mobCanvas.SetOwner(this, typeName);

            mobCircle.gameObject.SetActive(true);
            mobCanvas.gameObject.SetActive(true);

            mobCircle.transform.localScale = Vector3.one;

            mobStrategyInstance = ScriptableObject.CreateInstance(defaultMobStrategy.GetType()) as IMobStrategy;
            mobStrategyInstance.Init(this, agent, playerController);

            ResetState();
        }

        private void ReturnToPool()
        {
            PoolSystem.ReturnToPool(this);
        }
        #endregion
    }

    public enum BotState
    {
        Sleeping = 0,
        Idle = 1,
        Patrolling = 2,
        FollowPlayer = 3,
        Attacking = 4,
        Death = 5,
        Evade = 6,
        FindHelp = 7
    }

    public enum BotAnimatorState
    {
        Idle = 0,
        Walk = 1,
        Death = 2,
        Attack = 3
    }
}
