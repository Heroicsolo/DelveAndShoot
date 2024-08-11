using Heroicsolo.Heroicsolo.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Heroicsolo.Logics.Mobs
{
    [CreateAssetMenu(fileName = "New BasicMobStrategy", menuName = "Basic Mob Strategy", order = 51)]
    public class BasicMobStrategy : ScriptableObject, IMobStrategy
    {
        [SerializeField] [Min(0f)] protected float aggroConeAngle = 180f;
        [SerializeField] [Min(0f)] protected float aggroRadius = 10f;
        [SerializeField] protected bool canEvade = true;
        [ConditionalHide("canEvade", true, true)]
        [SerializeField] [Min(0f)] protected float evadeRadius = 20f;
        [SerializeField] [Min(0f)] protected float runAwayHpPercent = 0f;

        internal GenericMob owner;
        internal GenericMob nearestAlly;
        protected NavMeshAgent agent;
        protected PlayerController playerController;
        protected bool helpFound;

        public virtual void OnGetDamage(float damage, DamageType damageType = DamageType.Physical)
        {
            if (damage > 0f && (owner.BotState == BotState.Idle || owner.BotState == BotState.Sleeping))
            {
                SwitchState(BotState.FollowPlayer);
            }

            if (owner.StatsDict[CharacterStatType.Health].Value <= 0f)
            {
                SwitchState(BotState.Death);
            }
            else if (!helpFound && owner.StatsDict[CharacterStatType.Health].Percent < runAwayHpPercent)
            {
                SwitchState(BotState.FindHelp);
            }
        }

        public virtual void SwitchState(BotState state)
        {
            if (owner.BotState == state)
            {
                return;
            }

            owner.BotState = state;

            switch (state)
            {
                case BotState.Idle:
                    owner.StopMovement();
                    break;
                case BotState.Patrolling:
                    if (owner.PatrolPoints.Count > 0)
                    {
                        owner.SelectNextPatrolPoint();
                    }
                    else
                    {
                        owner.StopMovement();
                        owner.BotState = BotState.Idle;
                    }
                    break;
                case BotState.FollowPlayer:
                    owner.StartMovement(playerController.transform.position);
                    break;
                case BotState.Attacking:
                    owner.OnAggro();
                    owner.StopMovement();
                    owner.SetAnimatorState(BotAnimatorState.Attack);
                    break;
                case BotState.Death:
                    owner.Die();
                    break;
                case BotState.Evade:
                    owner.StartMovement(owner.SpawnPoint);
                    break;
                case BotState.FindHelp:
                    nearestAlly = owner.GetNearestTeamMember();
                    if (nearestAlly != null)
                    {
                        owner.StartMovement(nearestAlly.GetTransform().position);
                    }
                    else
                    {
                        SwitchState(BotState.Idle);
                    }
                    break;
            }
        }

        public virtual void UpdateCurrentState(float deltaTime)
        {
            switch (owner.BotState)
            {
                case BotState.Idle:
                    owner.StopMovement();
                    CheckPlayer();
                    break;
                case BotState.Patrolling:
                    if (owner.PatrolPoints.Count > 0 && owner.IsReachedNextPoint())
                    {
                        owner.SelectNextPatrolPoint();
                    }
                    CheckPlayer();
                    break;
                case BotState.FollowPlayer:
                    owner.StartMovement(playerController.transform.position);
                    CheckPlayer();
                    break;
                case BotState.Attacking:
                    owner.StopMovement();
                    owner.SetAnimatorState(BotAnimatorState.Attack);
                    CheckPlayer();
                    break;
                case BotState.Evade:
                    owner.StartMovement(owner.SpawnPoint);
                    if (owner.IsReachedNextPoint())
                    {
                        owner.ResetHealth();
                        owner.ResetState();
                    }
                    break;
                case BotState.FindHelp:
                    if (nearestAlly != null && !nearestAlly.IsDead())
                    {
                        owner.StartMovement(nearestAlly.GetTransform().position);

                        if (agent.remainingDistance < owner.AttackDistance)
                        {
                            helpFound = true;
                            nearestAlly.FollowPlayer();
                            owner.FollowPlayer();
                        }
                    }
                    else
                    {
                        nearestAlly = owner.GetNearestTeamMember();

                        if (nearestAlly == null)
                        {
                            SwitchState(BotState.Idle);
                        }
                        else
                        {
                            owner.StartMovement(nearestAlly.GetTransform().position);
                        }
                    }
                    break;
            }
        }

        void IMobStrategy.Init(GenericMob mob, NavMeshAgent mobAgent, PlayerController playerController)
        {
            owner = mob;
            agent = mobAgent;
            this.playerController = playerController;

            SetupAfterInit();
        }

        public virtual void SetupAfterInit()
        {
            helpFound = false;
        }

        protected virtual void CheckPlayer()
        {
            if (playerController != null && !playerController.IsDead())
            {
                float dist = Vector3.Distance(owner.transform.position, playerController.transform.position);

                bool inCone = VectorUtils.IsObjectInCone(playerController.transform, owner.transform, aggroConeAngle);

                if (dist < owner.AttackDistance)
                {
                    SwitchState(BotState.Attacking);
                    Vector3 lookPos = playerController.transform.position;
                    lookPos.y = owner.transform.position.y;
                    owner.transform.LookAt(lookPos);
                }
                else if (dist < aggroRadius && inCone)
                {
                    SwitchState(BotState.FollowPlayer);
                }
                else if (canEvade && owner.BotState == BotState.FollowPlayer
                    && (dist > evadeRadius || Vector3.Distance(owner.transform.position, owner.SpawnPoint) > evadeRadius))
                {
                    SwitchState(BotState.Evade);
                }
                else if (owner.BotState != BotState.FollowPlayer)
                {
                    owner.ResetState();
                }
            }
            else if (canEvade)
            {
                SwitchState(BotState.Evade);
            }
            else
            {
                SwitchState(BotState.Idle);
            }
        }
    }
}