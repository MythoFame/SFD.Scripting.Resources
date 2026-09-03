using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Fires a callback whenever a player is killed by another player. Wraps the built-in
    /// <see cref="Events.PlayerDamageCallback"/> and <see cref="Events.PlayerDeathCallback"/>
    /// to track the last attacker (via melee source or projectile's initial owner) and report
    /// them at the moment of death. Mirrors the lifecycle of the built-in SFD callbacks
    /// (exposed via the legacy <c>Events.*Callback</c> format): call <see cref="Start"/> to
    /// register, <see cref="Stop()"/> to unsubscribe.
    /// </summary>
    public class PlayerKillCallback : Events.CallbackDelegate
    {
        private readonly Action<IPlayer, IPlayer> _onKilledPlayer;
        private readonly Events.PlayerDamageCallback _playerDamageCallback;
        private readonly Events.PlayerDeathCallback _playerDeathCallback;

        private IPlayer lastAttacker = null;

        private PlayerKillCallback(Action<IPlayer, IPlayer> onKilledPlayer)
        {
            _onKilledPlayer = onKilledPlayer;

            _playerDamageCallback = Game.Events.StartPlayerDamageCallback(OnPlayerDamage);
            _playerDeathCallback = Game.Events.StartPlayerDeathCallback(OnPlayerDeath);
        }

        /// <summary>
        /// Starts monitoring for player kills and registers <paramref name="func"/> to be
        /// invoked with the killed and killer players whenever a kill occurs.
        /// </summary>
        /// <param name="func">
        /// The action to run on a kill; the first argument is the killed player, the second the killer.
        /// </param>
        /// <returns>The active <see cref="PlayerKillCallback"/> instance.</returns>
        public static PlayerKillCallback Start(Action<IPlayer, IPlayer> func) => new(func);

        /// <summary>
        /// Stops the given callback, unsubscribing from both damage and death events.
        /// </summary>
        /// <returns><c>true</c> if both underlying callbacks were successfully stopped.</returns>
        public static bool Stop(PlayerKillCallback callback) => callback._playerDamageCallback.Stop()
            && callback._playerDeathCallback.Stop();

        /// <summary>
        /// Invokes the registered kill action manually with the given players.
        /// </summary>
        /// <param name="killed">The player who was killed.</param>
        /// <param name="killer">The player responsible for the kill.</param>
        public void Invoke(IPlayer killed, IPlayer killer) => _onKilledPlayer(killed, killer);

        /// <inheritdoc/>
        public override void Dispose() => lastAttacker = null;

        /// <inheritdoc/>
        public override bool Stop() => _playerDamageCallback.Stop() && _playerDeathCallback.Stop();

        private void OnPlayerDamage(IPlayer player, PlayerDamageArgs args) => lastAttacker = (object)args.DamageType switch
        {
            PlayerDamageEventType.Melee => Game.GetPlayer(args.SourceID),
            PlayerDamageEventType.Projectile => Game.GetPlayer(Game.GetProjectile(args.SourceID)
                                .InitialOwnerPlayerID),
            _ => null,
        };

        private void OnPlayerDeath(IPlayer player, PlayerDeathArgs args)
        {
            if (lastAttacker != null && player != null)
                _onKilledPlayer(player, lastAttacker);
        }
    }
}
