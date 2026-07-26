using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Polls <see cref="IGame.IsGameOver"/> on an update loop and fires the supplied
    /// callback once when the game ends, then stops itself. Mirrors the lifecycle of the
    /// built-in SFD callbacks (exposed via the legacy <c>Events.*Callback</c> format):
    /// call <see cref="Start"/> to register, <see cref="Stop()"/> to unsubscribe.
    /// </summary>
    public class GameOverCallback : Events.CallbackDelegate
    {
        private const uint COOLDOWN = 50;

        private readonly Events.UpdateCallback _updateCallback;
        private readonly Action _onGameOver;

        private GameOverCallback(Action onGameOver)
        {
            _updateCallback = Game.Events.StartUpdateCallback(Update, COOLDOWN);
            _onGameOver = onGameOver;
        }

        /// <summary>
        /// Starts polling for game over and registers <paramref name="func"/> to be
        /// invoked once when <see cref="IGame.IsGameOver"/> becomes <c>true</c>.
        /// </summary>
        /// <param name="func">The action to run when the game ends.</param>
        /// <returns>The active <see cref="GameOverCallback"/> instance.</returns>
        public static GameOverCallback Start(Action func) => new(func);

        /// <summary>
        /// Stops the given callback, halting its update loop.
        /// </summary>
        /// <returns><c>true</c> if the callback was successfully stopped.</returns>
        public static bool Stop(GameOverCallback callback) => callback._updateCallback.Stop();

        /// <summary>
        /// Invokes the registered game-over action manually.
        /// </summary>
        public void Invoke() => _onGameOver();

        /// <inheritdoc/>
        public override void Dispose() { }

        /// <inheritdoc/>
        public override bool Stop() => _updateCallback.Stop();

        private void Update(float dlt)
        {
            if (Game.IsGameOver)
            {
                Invoke();

                Stop();
            }
        }
    }
}
