using Heroicsolo.DI;
using Heroicsolo.Scripts.Player;

namespace Heroicsolo.Scripts.Logics
{
    public interface IGameController : ISystem
    {
        PlayerController GetPlayerController();
    }
}