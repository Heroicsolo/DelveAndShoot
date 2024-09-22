using Heroicsolo.DI;
using Heroicsolo.Heroicsolo.Player;

namespace Heroicsolo.Logics
{
    public interface IGameController : ISystem
    {
        PlayerController GetPlayerController();
        void ShakeCamera(float strength = 1f, float timeLength = 1f);
        void LevelCompleted();
    }
}