namespace Dodoco.Core.Game;

public static class GameUpdateManagerFactory {

    public static IGameUpdateManager Create(IGame game) => new GameUpdateManager(game);

}