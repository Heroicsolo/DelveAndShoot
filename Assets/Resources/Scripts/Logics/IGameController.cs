using Heroicsolo.DI;
using Heroicsolo.Heroicsolo.Player;

namespace Heroicsolo.Logics
{
    public interface IGameController : ISystem
    {
        PlayerController GetPlayerController();
    }
}