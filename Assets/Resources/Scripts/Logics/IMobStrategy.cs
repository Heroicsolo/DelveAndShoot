using Assets.Resources.Scripts.Logics;
using Heroicsolo.Scripts.Logics;
using Heroicsolo.Scripts.Player;
using UnityEngine.AI;

namespace Heroicsolo.Logics
{
    public interface IMobStrategy
    {
        internal void Init(GenericMob mob, NavMeshAgent mobAgent, PlayerController playerController);
        void UpdateCurrentState(float deltaTime);
        void SwitchState(BotState state);
        void OnGetDamage(float damage, DamageType damageType = DamageType.Physical);
    }
}