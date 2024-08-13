using UnityEngine;

namespace Heroicsolo.Logics.Mobs
{
    [CreateAssetMenu(fileName = "New StationaryMobStrategy", menuName = "Stationary Mob Strategy", order = 52)]
    public class StationaryMobStrategy : BasicMobStrategy
    {
        public override void SwitchState(BotState state)
        {
            if (owner.BotState == state)
            {
                return;
            }

            owner.BotState = state;

            switch (state)
            {
                case BotState.Idle:
                case BotState.Patrolling:
                    owner.StopMovement();
                    break;
                case BotState.FollowPlayer:
                case BotState.Attacking:
                    owner.OnAggro();
                    owner.StopMovement();
                    owner.SetAnimatorState(BotAnimatorState.Attack);
                    break;
                case BotState.Death:
                    owner.Die();
                    break;
                case BotState.Evade:
                    owner.ResetHealth();
                    owner.ResetState();
                    SwitchState(BotState.Idle);
                    break;
                case BotState.FindHelp:
                    nearestAlly = owner.GetNearestTeamMember();
                    if (nearestAlly != null)
                    {
                        helpFound = true;
                        nearestAlly.FollowPlayer();
                    }
                    else
                    {
                        SwitchState(BotState.Idle);
                    }
                    break;
            }
        }

        public override void UpdateCurrentState(float deltaTime)
        {
            switch (owner.BotState)
            {
                case BotState.Idle:
                case BotState.Patrolling:
                    owner.StopMovement();
                    CheckPlayer();
                    break;
                case BotState.FollowPlayer:
                case BotState.Attacking:
                    owner.StopMovement();
                    owner.SetAnimatorState(BotAnimatorState.Attack);
                    CheckPlayer();
                    break;
                case BotState.Evade:
                    break;
                case BotState.FindHelp:
                    if (!helpFound && nearestAlly != null && !nearestAlly.IsDead())
                    {
                        helpFound = true;
                        nearestAlly.FollowPlayer();
                        SwitchState(BotState.FollowPlayer);
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
                            helpFound = true;
                            nearestAlly.FollowPlayer();
                            SwitchState(BotState.FollowPlayer);
                        }
                    }
                    break;
            }
        }

        public override void SetupAfterInit()
        {
            base.SetupAfterInit();
            agent.enabled = false;
        }
    }
}