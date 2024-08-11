using Heroicsolo.Logics;
using System;
using System.Collections.Generic;

namespace Heroicsolo.Logics
{
    public interface ICharacter : IHittable
    {
        CharacterStat GetCharacterStat(CharacterStatType characterStatType);
        List<CharacterStat> GetCharacterStats();
        void SubscribeToDamageGot(Action<float> onDamageGot);
        void SubscribeToDamageDodged(Action onDamageDodged);
    }
}