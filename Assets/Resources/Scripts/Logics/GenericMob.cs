using Heroicsolo.DI;
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

        [Inject] private ICharacterStatsManager characterStatsManager;

        private string _typeName;
        private NavMeshAgent agent;
        private Animator animator;
        private BotState botState;
        private Dictionary<CharacterStatType, CharacterStat> statsDict = new Dictionary<CharacterStatType, CharacterStat>();
        private PlayerController playerController;
        private Transform lastPatrolPoint;
        private TeamType currentTeam;

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

            botState = BotState.Sleeping;

            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
        }

        private void InitState()
        {
            statsDict.Clear();

            foreach (CharacterStat characterStat in stats)
            {
                statsDict.Add(characterStat.StatType, characterStat);
            }

            statsDict[CharacterStatType.Health].SetRegenState(true);

            playerController = FindObjectOfType<PlayerController>();

            lastPatrolPoint = null;

            currentTeam = defaultTeam;

            SwitchState(BotState.Idle);
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

        private void UpdateCurrentState()
        {
            switch (botState)
            {
                case BotState.Idle:
                    agent.isStopped = true;
                    break;
                case BotState.Patrolling:
                    if (patrolPoints.Count > 0 && IsReachedNextPatrolPoint())
                    {
                        SelectNextPatrolPoint();
                        agent.isStopped = false;
                        animator.SetBool(WalkAnimHash, true);
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
                    break;
                case BotState.Death:
                    agent.isStopped = true;
                    break;
            }
        }

        private void Update()
        {
            UpdateCurrentState();
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
                    break;
                case BotState.Death:
                    agent.isStopped = true;
                    animator.SetTrigger(DieAnimHash);
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
        Death = 5
    }
}
