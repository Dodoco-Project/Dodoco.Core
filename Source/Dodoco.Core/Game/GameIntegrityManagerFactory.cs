namespace Dodoco.Core.Game;

public static class GameIntegrityManagerFactory {

    public static IGameIntegrityManager Create(IGame game) => new GameIntegrityManager(game);

}