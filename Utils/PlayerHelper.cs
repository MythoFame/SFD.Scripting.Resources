using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Provides convenience helpers for querying and manipulating <see cref="IPlayer"/> instances.
    /// </summary>
    public static class PlayerHelper
    {
        private static readonly Vector2 _stickyFeetTransition = new(0, 2);

        /// <summary>
        /// Nudges a player upward by a small offset, useful for freeing them when they are
        /// stuck on terrain edges or inside geometry.
        /// </summary>
        public static void Unstick(IPlayer player) => player.SetWorldPosition(player.GetWorldPosition() + _stickyFeetTransition);

        /// <summary>
        /// Determines whether a player is currently firing by manually aiming, accounting for
        /// the relevant drawn weapon and its remaining ammo. Returns false when the player is
        /// not manual-aiming, not pressing attack, or has no ammo for the drawn weapon.
        /// </summary>
        public static bool IsManualFiring(IPlayer player)
        {
            if (!player.IsManualAiming || !player.KeyPressed(VirtualKey.ATTACK))
                return false;

            return player.CurrentWeaponDrawn switch
            {
                WeaponItemType.Handgun => player.CurrentSecondaryRangedWeapon.TotalAmmo > 0,
                WeaponItemType.Rifle => player.CurrentPrimaryRangedWeapon.TotalAmmo > 0,
                _ => false
            };
        }

        /// <summary>
        /// Determines whether a player is currently firing in any form, including hip-firing
        /// and manual aiming (see <see cref="IsManualFiring"/>).
        /// </summary>
        public static bool IsFiring(IPlayer player) => player.IsHipFiring || IsManualFiring(player);

        /// <summary>
        /// Determines whether the player is dodging and can be hit by projectiles.
        /// </summary>
        public static bool IsDodging(IPlayer player) => player.IsRolling || player.IsDiving;

        /// <summary>
        /// Gets whether a player currently has a ranged weapon, whether it be the
        /// primary or secondary weapon slot.
        /// </summary>
        public static bool HasRangedWeapon(IPlayer player) => player.CurrentPrimaryWeapon.WeaponItem != WeaponItem.NONE ||
          player.CurrentSecondaryWeapon.WeaponItem != WeaponItem.NONE;

        /// <summary>
        /// Determines whether two players are enemies based on their teams. Players on
        /// <see cref="PlayerTeam.Independent"/> are enemies of everyone; otherwise, two
        /// players are enemies unless they belong to the same team.
        /// </summary>
        /// <param name="player">The first player.</param>
        /// <param name="other">The player to compare against.</param>
        /// <returns>Whether the players are enemies.</returns>
        public static bool IsEnemy(IPlayer player, IPlayer other) => IsEnemy(player.GetTeam(), other.GetTeam());

        /// <summary>
        /// Determines whether two teams are enemies. <see cref="PlayerTeam.Independent"/> is an enemy of everyone;
        /// otherwise, two teams are enemies unless they are the same.
        /// </summary>
        /// <param name="team">The first team.</param>
        /// <param name="otherTeam">The second team.</param>
        /// <returns>Whether the teams are enemies.</returns>
        /// <remarks>
        /// If you are trying to check if two <see cref="IPlayer"/> instances are enemies, use <see cref="IsEnemy(IPlayer,IPlayer)"/>
        /// instead.
        /// </remarks>
        public static bool IsEnemy(PlayerTeam team, PlayerTeam otherTeam) => team == PlayerTeam.Independent ||
            otherTeam == PlayerTeam.Independent || team != otherTeam;

        /// <summary>
        /// Gets the current sound effect that should be run for a projectile hit effect given the hit effect type.
        /// </summary>
        public static string GetHitEffectProjSoundEffect(IPlayer player) => GetHitEffectProjSoundEffect(player.GetHitEffect());

        /// <summary>
        /// Gets the current particle effect name to be used for a given hit effect.
        /// </summary>
        public static string GetHitEffectName(IPlayer player) => GetHitEffectName(player.GetHitEffect());

        /// <summary>
        /// Gets the current sound effect that should be run for a projectile hit effect given the hit effect type.
        /// </summary>
        public static string GetHitEffectProjSoundEffect(PlayerHitEffect hitEffect) => hitEffect == PlayerHitEffect.Default ? "BulletHitFlesh" : "BulletHitMetal";

        /// <summary>
        /// Gets the current particle effect name to be used for a given hit effect.
        /// </summary>
        public static string GetHitEffectName(PlayerHitEffect hitEffect) => hitEffect == PlayerHitEffect.Default ? EffectName.Blood : EffectName.BulletHitMetal;

        /// <summary>
        /// Issues a command to a player immediately. If the player's input is currently
        /// enabled, it is disabled for the command and re-enabled one update later.
        /// </summary>
        /// <param name="player">The player to issue the command to.</param>
        /// <param name="command">The command to execute.</param>
        public static void QuickCommand(IPlayer player, PlayerCommand command)
        {
            bool inputEnabled = player.IsInputEnabled;

            player.AddCommand(command);

            if (inputEnabled)
            {
                player.SetInputEnabled(false);

                Game.Events.StartUpdateCallback(dlt =>
                {
                    player?.SetInputEnabled(true);
                }, 1, 1);
            }
        }

        /// <summary>
        /// Issues a command of the given type to a player immediately. If the player's
        /// input is currently enabled, it is disabled for the command and re-enabled one
        /// update later.
        /// </summary>
        /// <param name="player">The player to issue the command to.</param>
        /// <param name="commandType">The type of command to execute.</param>
        public static void QuickCommand(IPlayer player, PlayerCommandType commandType) => QuickCommand(player, new PlayerCommand(commandType));

        /// <summary>
        /// Revives a dead player by recreating them at their death position with the same
        /// user (including bot behavior for bots), profile, team and input mode, then
        /// removes the original player. Does nothing when the player is not dead.
        /// </summary>
        /// <param name="player">The dead player to revive.</param>
        /// <returns>The recreated player, or <c>null</c> when the player is not dead.</returns>
        public static IPlayer Revive(IPlayer player)
        {
            if (!player.IsDead)
            {
                return null;
            }

            IPlayer revived = Game.CreatePlayer(player.GetWorldPosition());

            IUser user = player.GetUser();

            if (user != null)
            {
                if (user.IsBot)
                {
                    revived.SetBotBehavior(new BotBehavior(true, user.BotPredefinedAIType));
                }

                revived.SetUser(user);
            }

            revived.SetProfile(player.GetProfile());
            revived.SetTeam(player.GetTeam());
            revived.SetInputMode(player.InputMode);

            player.Remove();

            return revived;
        }
    }
}
