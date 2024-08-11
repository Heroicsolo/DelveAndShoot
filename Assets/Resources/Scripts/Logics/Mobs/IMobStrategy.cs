using Heroicsolo.Heroicsolo.Player;
using UnityEngine.AI;

namespace Heroicsolo.Logics.Mobs
{
    public interface IMobStrategy
    {
        internal void Init(GenericMob mob, NavMeshAgent mobAgent, PlayerController playerController);
        void UpdateCurrentState(float deltaTime);
        void SwitchState(BotState state);
        void OnGetDamage(float damage, DamageType damageType = DamageType.Physical);
    }
}