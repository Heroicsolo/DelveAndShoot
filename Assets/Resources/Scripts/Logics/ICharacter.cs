using Heroicsolo.Scripts.Logics;
using System.Collections.Generic;

namespace Heroicsolo.Logics
{
    public interface ICharacter : IHittable
    {
        CharacterStat GetCharacterStat(CharacterStatType characterStatType);
        List<CharacterStat> GetCharacterStats();
    }
}