using Heroicsolo.DI;
using Heroicsolo.Logics;

namespace Heroicsolo.Logics
{
    public interface ICharacterStatsManager : ISystem
    {
        void ModifyCharacterStat(ICharacter character, CharacterStatModifier modifier);
        float GetDamageAbsorbPercentage(float armorAmount);
    }
}