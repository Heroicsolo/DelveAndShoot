using Heroicsolo.DI;
using System;

namespace Heroicsolo.Scripts.Logics
{
    public interface IPlayerProgressionManager : ISystem
    {
        (int, int, int) GetPlayerLevelState();
        void AddExperience(int amount);
        int GetNeededExpForLevelUp();
        int GetCurrentLevelMaxExp();
        void SubscribeToLevelUpEvent(Action<int> onLevelUpCallback);
        void UnsubscribeFromLevelUpEvent(Action<int> onLevelUpCallback);
        void SubscribeToExpChangeEvent(Action<int, int, int> onExperienceChanged);
        void UnsubscribeFromExpChangeEvent(Action<int, int, int> onExperienceChanged);
    }
}