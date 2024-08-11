using Assets.Resources.Scripts.Logics;
using Heroicsolo.Scripts.Logics;
using Heroicsolo.Scripts.Player;
using UnityEngine;
using UnityEngine.AI;

namespace Heroicsolo.Logics
{
    public class BasicMobStrategy : IMobStrategy
    {
        private GenericMob owner;
        private NavMeshAgent agent;
        private PlayerController playerController;
        private GenericMob nearestAlly;
        private bool helpFound;

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
            else if (!helpFound && owner.StatsDict[CharacterStatType.Health].Percent < owner.RunAwayHPPercent)
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
            helpFound = false;
            this.playerController = playerController;
        }

        private void CheckPlayer()
        {
            if (playerController != null && !playerController.IsDead())
            {
                float dist = Vector3.Distance(owner.transform.position, playerController.transform.position);

                bool inCone = VectorUtils.IsObjectInCone(playerController.transform, owner.transform, owner.AggroConeAngle);

                if (dist < owner.AttackDistance)
                {
                    SwitchState(BotState.Attacking);
                    Vector3 lookPos = playerController.transform.position;
                    lookPos.y = owner.transform.position.y;
                    owner.transform.LookAt(lookPos);
                }
                else if (dist < owner.AggroRadius && inCone)
                {
                    SwitchState(BotState.FollowPlayer);
                }
                else if (owner.BotState == BotState.FollowPlayer
                    && (dist > owner.EvadeRadius || Vector3.Distance(owner.transform.position, owner.SpawnPoint) > owner.EvadeRadius))
                {
                    SwitchState(BotState.Evade);
                }
                else if (owner.BotState != BotState.FollowPlayer)
                {
                    owner.ResetState();
                }
            }
            else
            {
                SwitchState(BotState.Evade);
            }
        }
    }
}