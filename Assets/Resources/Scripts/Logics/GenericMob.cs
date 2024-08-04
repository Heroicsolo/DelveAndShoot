using DG.Tweening;
using Heroicsolo.DI;
using Heroicsolo.Inventory;
using Heroicsolo.Logics;
using Heroicsolo.Scripts.Logics;
using Heroicsolo.Scripts.Player;
using Heroicsolo.Scripts.UI;
using Heroicsolo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Assets.Resources.Scripts.Logics
{
    /// <summary>
    /// NOT FULLY IMPLEMENTED
    /// </summary>
    internal class GenericMob : Mob
    {
        private readonly int WalkAnimHash = Animator.StringToHash("Walk");
        private readonly int AttackAnimHash = Animator.StringToHash("Attack");
        private readonly int DieAnimHash = Animator.StringToHash("Die");
        private const float TargetReachThreshold = 0.2f;

        [SerializeField] private string typeName;
        [SerializeField] private HittableType creatureType;
        [SerializeField] private TeamType defaultTeam;
        [SerializeField] private List<CharacterStat> stats = new();
        [SerializeField] private List<Transform> patrolPoints = new();
        [SerializeField] [Min(0f)] private float attackDistance = 3f;
        [SerializeField] [Min(0f)] private float aggroRadius = 10f;
        [SerializeField] [Min(0f)] private float evadeRadius = 20f;
        [SerializeField] [Min(0f)] private float dissapearTime = 5f;
        [SerializeField] [Min(0f)] private float attackPowerMin = 1f;
        [SerializeField] [Min(0f)] private float attackPowerMax = 1f;
        [SerializeField] [Min(0)] private int expReward = 100;
        [SerializeField] private string lootId;
        [SerializeField] private MobCanvas mobCanvas;
        [SerializeField] private Transform mobCircle;

        [Inject] private ICharacterStatsManager characterStatsManager;
        [Inject] private IPlayerProgressionManager playerProgressionManager;
        [Inject] private ILootManager lootManager;
        [Inject] private ITeamsManager teamsManager;

        private string _typeName;
        private NavMeshAgent agent;
        private Animator animator;
        private BotState botState;
        private Dictionary<CharacterStatType, CharacterStat> statsDict = new Dictionary<CharacterStatType, CharacterStat>();
        private PlayerController playerController;
        private Transform lastPatrolPoint;
        private TeamType currentTeam;
        private Vector3 spawnPoint;
        private bool weaponActive;
        private Action<float> OnDamageGot;
        private Action OnDamageDodged;

        public float AttackDamage => UnityEngine.Random.Range(attackPowerMin, attackPowerMax);

        private void Start()
        {
            SystemsManager.InjectSystemsTo(this);

#if UNITY_EDITOR
            if(string.IsNullOrEmpty(typeName))
            {
                Debug.LogError($"Mob prefab {gameObject.name} spawned without proper pool name assigned");
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif
            _typeName = typeName;

            weaponActive = false;
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

            if (!agent.isOnNavMesh || !agent.isActiveAndEnabled)
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

            ResetState();
        }

        private void SelectNextPatrolPoint()
        {
            Transform nextPoint = lastPatrolPoint != null ? patrolPoints.GetRandomElementExceptOne(lastPatrolPoint) : patrolPoints.GetRandomElement();

            StartMovement(nextPoint.position);

            lastPatrolPoint = nextPoint;
        }

        private bool IsReachedNextPatrolPoint()
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

        private void ResetState()
        {
            if (patrolPoints.Count > 0)
            {
                SwitchState(BotState.Patrolling);
            }
            else
            {
                SwitchState(BotState.Idle);
            }
        }

        private void CheckPlayer()
        {
            if (playerController != null && !playerController.IsDead())
            {
                float dist = Vector3.Distance(transform.position, playerController.transform.position);

                if (dist < attackDistance)
                {
                    SwitchState(BotState.Attacking);
                }
                else if (dist < aggroRadius)
                {
                    SwitchState(BotState.FollowPlayer);
                }
                else if (botState == BotState.FollowPlayer 
                    && (dist > evadeRadius || Vector3.Distance(transform.position, spawnPoint) > evadeRadius))
                {
                    SwitchState(BotState.Evade);
                }
                else
                {
                    ResetState();
                }
            }
            else
            {
                SwitchState(BotState.Evade);
            }
        }

        private void UpdateCurrentState()
        {
            switch (botState)
            {
                case BotState.Idle:
                    StopMovement();
                    CheckPlayer();
                    break;
                case BotState.Patrolling:
                    if (patrolPoints.Count > 0 && IsReachedNextPatrolPoint())
                    {
                        SelectNextPatrolPoint();
                    }
                    CheckPlayer();
                    break;
                case BotState.FollowPlayer:
                    StartMovement(playerController.transform.position);
                    CheckPlayer();
                    break;
                case BotState.Attacking:
                    StopMovement();
                    animator.SetBool(AttackAnimHash, true);
                    CheckPlayer();
                    break;
                case BotState.Evade:
                    StartMovement(spawnPoint);
                    if (IsReachedNextPatrolPoint())
                    {
                        statsDict[CharacterStatType.Health].Reset();
                        ResetState();
                    }
                    break;
            }
        }

        private void Update()
        {
            if (botState != BotState.Death)
            {
                UpdateCurrentState();
            }
        }

        private void SpawnLoot()
        {
            lootManager.GenerateRandomDrop(lootId, transform.position);
        }

        private void ReturnToPool()
        {
            PoolSystem.ReturnToPool(this);
        }

        private void StopMovement()
        {
            if (agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.isStopped = true;
            }

            animator.SetBool(WalkAnimHash, false);
        }

        private void StartMovement(Vector3 destination)
        {
            if (agent.isOnNavMesh && agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
                agent.SetDestination(destination);
                animator.SetBool(WalkAnimHash, true);
                animator.SetBool(AttackAnimHash, false);
            }
        }

        public override void SubscribeToDamageGot(Action<float> onDamageGot)
        {
            OnDamageGot += onDamageGot;
        }

        public override void SubscribeToDamageDodged(Action onDamageDodged)
        {
            OnDamageDodged += onDamageDodged;
        }

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
            
        public bool IsDamageable()
        {
            return botState != BotState.Evade && botState != BotState.Death && statsDict[CharacterStatType.Health].Value > 0f;
        }

        public override void Activate()
        {
            InitState();
        }

        public override void Deactivate()
        {
            SwitchState(BotState.Sleeping);
        }

        public override void Die()
        {
            playerProgressionManager.AddExperience(expReward);
            SwitchState(BotState.Death);
        }

        public override List<CharacterStat> GetCharacterStats()
        {
            return stats;
        }

        public override CharacterStat GetCharacterStat(CharacterStatType characterStatType)
        {
            return stats.Find(s => s.StatType == characterStatType);
        }

        public override void DodgeDamage()
        {
            OnDamageDodged?.Invoke();
        }

        public override void GetDamage(float damage, DamageType damageType = DamageType.Physical)
        {
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

            if (statsDict[CharacterStatType.Health].Value <= 0f)
            {
                Die();
            }
        }

        public override GameObject GetGameObject()
        {
            return gameObject;
        }

        public override HittableType GetHittableType()
        {
            return creatureType;
        }

        public override string GetName()
        {
            return _typeName;
        }

        public override TeamType GetTeamType()
        {
            return currentTeam;
        }

        public override Transform GetTransform()
        {
            return transform;
        }

        public override void Heal(float amount)
        {
            statsDict[CharacterStatType.Health].Change(amount);
        }

        public override bool IsDead()
        {
            return statsDict[CharacterStatType.Health].Value <= 0f;
        }

        public override void SetName(string name)
        {
            _typeName = name;
        }

        public override void SetTeam(TeamType team)
        {
            currentTeam = team;
        }

        public void SwitchState(BotState state)
        {
            if (botState == state)
            {
                return;
            }

            botState = state;

            switch (state)
            {
                case BotState.Idle:
                    StopMovement();
                    break;
                case BotState.Patrolling:
                    if (patrolPoints.Count > 0)
                    {
                        SelectNextPatrolPoint();
                    }
                    else
                    {
                        StopMovement();
                        botState = BotState.Idle;
                    }
                    break;
                case BotState.FollowPlayer:
                    StartMovement(playerController.transform.position);
                    break;
                case BotState.Attacking:
                    StopMovement();
                    animator.SetBool(AttackAnimHash, true);
                    break;
                case BotState.Death:
                    StopMovement();
                    animator.SetBool(AttackAnimHash, false);
                    animator.SetTrigger(DieAnimHash);
                    mobCircle.gameObject.SetActive(false);
                    mobCanvas.gameObject.SetActive(false);
                    SpawnLoot();
                    Invoke(nameof(ReturnToPool), dissapearTime);
                    break;
                case BotState.Evade:
                    StartMovement(spawnPoint);
                    break;
            }
        }
    }

    public enum BotState
    {
        Sleeping = 0,
        Idle = 1,
        Patrolling = 2,
        FollowPlayer = 3,
        Attacking = 4,
        Death = 5,
        Evade = 6
    }
}
