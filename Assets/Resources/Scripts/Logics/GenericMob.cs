using Heroicsolo.DI;
using Heroicsolo.Inventory;
using Heroicsolo.Logics;
using Heroicsolo.Scripts.Logics;
using Heroicsolo.Scripts.Player;
using Heroicsolo.Scripts.UI;
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
        [SerializeField] private string lootId;

        [Inject] private ICharacterStatsManager characterStatsManager;
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

            playerController = FindObjectOfType<PlayerController>();

            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            lastPatrolPoint = null;

            currentTeam = defaultTeam;

            teamsManager.RegisterTeamMember(this, currentTeam);

            spawnPoint = transform.position;

            ResetState();
        }

        private void SelectNextPatrolPoint()
        {
            Transform nextPoint = lastPatrolPoint != null ? patrolPoints.GetRandomElementExceptOne(lastPatrolPoint) : patrolPoints.GetRandomElement();

            agent.SetDestination(nextPoint.position);

            lastPatrolPoint = nextPoint;
        }

        private bool IsReachedNextPatrolPoint()
        {
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
                else if (dist > evadeRadius)
                {
                    SwitchState(BotState.Evade);
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
                    agent.isStopped = true;
                    CheckPlayer();
                    break;
                case BotState.Patrolling:
                    if (patrolPoints.Count > 0 && IsReachedNextPatrolPoint())
                    {
                        SelectNextPatrolPoint();
                        agent.isStopped = false;
                        animator.SetBool(WalkAnimHash, true);
                    }
                    CheckPlayer();
                    break;
                case BotState.FollowPlayer:
                    agent.isStopped = false;
                    agent.SetDestination(playerController.transform.position);
                    animator.SetBool(WalkAnimHash, true);
                    CheckPlayer();
                    break;
                case BotState.Attacking:
                    agent.isStopped = true;
                    animator.SetBool(WalkAnimHash, false);
                    animator.SetBool(AttackAnimHash, true);
                    CheckPlayer();
                    break;
                case BotState.Death:
                    agent.isStopped = true;
                    break;
                case BotState.Evade:
                    agent.isStopped = false;
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
            UpdateCurrentState();
        }

        private void SpawnLoot()
        {
            lootManager.GenerateRandomDrop(lootId, transform.position);
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

        public override void GetDamage(float damage, DamageType damageType = DamageType.Physical)
        {
            if (!IsDamageable())
            {
                return;
            }

            if (statsDict.ContainsKey(CharacterStatType.Armor) && damageType == DamageType.Physical)
            {
                damage *= (1f - characterStatsManager.GetDamageAbsorbPercentage(statsDict[CharacterStatType.Armor].Value));
            }

            statsDict[CharacterStatType.Health].Change(-damage);
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
                    agent.isStopped = true;
                    animator.SetBool(WalkAnimHash, false);
                    break;
                case BotState.Patrolling:
                    if (patrolPoints.Count > 0)
                    {
                        SelectNextPatrolPoint();
                        agent.isStopped = false;
                        animator.SetBool(WalkAnimHash, true);
                    }
                    else
                    {
                        agent.isStopped = true;
                        animator.SetBool(WalkAnimHash, false);
                        botState = BotState.Idle;
                    }
                    break;
                case BotState.FollowPlayer:
                    agent.isStopped = false;
                    agent.SetDestination(playerController.transform.position);
                    animator.SetBool(WalkAnimHash, true);
                    break;
                case BotState.Attacking:
                    agent.isStopped = true;
                    animator.SetBool(WalkAnimHash, false);
                    animator.SetBool(AttackAnimHash, true);
                    break;
                case BotState.Death:
                    agent.isStopped = true;
                    animator.SetBool(WalkAnimHash, false);
                    animator.SetTrigger(DieAnimHash);
                    SpawnLoot();
                    break;
                case BotState.Evade:
                    agent.isStopped = false;
                    agent.SetDestination(spawnPoint);
                    animator.SetBool(WalkAnimHash, true);
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
