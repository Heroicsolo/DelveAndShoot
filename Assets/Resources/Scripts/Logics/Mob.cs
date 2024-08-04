using Heroicsolo.Logics;
using Heroicsolo.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Heroicsolo.Scripts.Logics
{
    internal abstract class Mob : MonoBehaviour, IPooledObject, ICharacter
    {
        public abstract void Activate();
        public abstract void Deactivate();
        public abstract void Die();
        public abstract CharacterStat GetCharacterStat(CharacterStatType characterStatType);
        public abstract List<CharacterStat> GetCharacterStats();
        public abstract void GetDamage(float damage, DamageType damageType = DamageType.Physical);
        public abstract void DodgeDamage();
        public abstract GameObject GetGameObject();
        public abstract HittableType GetHittableType();
        public abstract string GetName();
        public abstract TeamType GetTeamType();
        public abstract Transform GetTransform();
        public abstract void Heal(float amount);
        public abstract bool IsDead();
        public abstract void SetName(string name);
        public abstract void SetTeam(TeamType team);
        public abstract void SubscribeToDamageGot(Action<float> onDamageGot);
        public abstract void SubscribeToDamageDodged(Action onDamageDodged);
    }
}
