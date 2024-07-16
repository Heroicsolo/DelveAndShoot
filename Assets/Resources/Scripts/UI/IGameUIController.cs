using Heroicsolo.DI;

namespace Heroicsolo.Scripts.UI
{
    public interface IGameUIController : ISystem
    {
        void OnHealthChanged(float oldVal, float newVal, float maxVal);
        void OnStaminaChanged(float oldVal, float newVal, float maxVal);
        void OnPlayerLevelUp(int level);
        void SetAmmo(int ammo);
        void RemoveAmmo();
    }
}