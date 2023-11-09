namespace Dodoco.Core.Game;

public static class GameFactory {

    public static IGame Create(GameSettings settings) => new Game(settings);

}