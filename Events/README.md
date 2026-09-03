# [GameOverCallback](GameOverCallback.cs)

Polls `Game.IsGameOver` on an update loop and fires the supplied callback once when the game ends, then stops itself. Useful for triggering end-of-match logic without manually checking the game state every update.

Example usage:

```cs
GameOverCallback _gameOverEvent = null;

public void OnStartup()
{
    _gameOverEvent = GameOverCallback.Start(OnGameOver);
}

public static void OnGameOver() => Game.WriteToConsoleF("Game over!");
```

# [PlayerKillCallback](PlayerKillCallback.cs)

Fires a callback whenever a player is killed by another player. Wraps the built-in `PlayerDamageCallback` and `PlayerDeathCallback` to track the last attacker (via melee source or projectile's initial owner) and report them at the moment of death.

Example usage:

```cs
PlayerKillCallback _killEvent = null;

public void OnStartup()
{
    _killEvent = PlayerKillCallback.Start(OnPlayerKill);
}

public static void OnPlayerKill(IPlayer killed, IPlayer killer) => Game.WriteToConsoleF($"{killer.Name} killed {killed.Name}!");
```
