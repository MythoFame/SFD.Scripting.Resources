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
        private static readonly Color _respawnColor = new(242, 157, 208);

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

            IUser user = player.GetUser();

            IPlayer revived = Game.CreatePlayer(player.GetWorldPosition());

            if (user != null)
            {
                if (user.IsBot)
                {
                    revived.SetBotBehavior(new(true, user.BotPredefinedAIType));
                }

                revived.SetUser(user);
            }

            revived.SetProfile(player.GetProfile());
            revived.SetTeam(player.GetTeam());
            revived.SetInputMode(player.InputMode);
            revived.SetBotName(player.Name);

            if (player.IsFalling)
            {
                revived.Fall();
            }

            revived.SetLinearVelocity(player.GetLinearVelocity());
            revived.SetAngularVelocity(player.GetAngularVelocity());

            revived.GiveWeaponItem(player.CurrentMeleeWeapon.WeaponItem);
            revived.SetCurrentMeleeDurability(player.CurrentMeleeWeapon.Durability);

            revived.GiveWeaponItem(player.CurrentSecondaryWeapon.WeaponItem);
            revived.SetCurrentSecondaryWeaponAmmo(player.CurrentSecondaryWeapon.TotalAmmo);

            revived.GiveWeaponItem(player.CurrentPrimaryWeapon.WeaponItem);
            revived.SetCurrentPrimaryWeaponAmmo(player.CurrentPrimaryWeapon.TotalAmmo);

            revived.GiveWeaponItem(player.CurrentThrownItem.WeaponItem);
            revived.SetCurrentThrownItemAmmo(player.CurrentThrownItem.CurrentAmmo);

            revived.GiveWeaponItem(player.CurrentPowerupItem.WeaponItem);

            player.Remove();

            return revived;
        }

        /// <summary>
        /// Spawns a player with the given user and position, using the user's profile and
        /// team, and sets the user for the spawned player.
        /// </summary>
        /// <param name="user">The user to spawn.</param>
        /// <param name="pos">The position to spawn at.</param>
        /// <returns>The spawned player.</returns>
        public static IPlayer Spawn(IUser user, Vector2 pos)
        {
            IPlayer spawned = Game.CreatePlayer(pos);

            if (user.IsBot)
            {
                spawned.SetBotBehavior(new(true, user.BotPredefinedAIType));
            }

            spawned.SetUser(user);

            spawned.SetProfile(user.GetProfile());
            spawned.SetTeam(user.GetTeam());

            return spawned;
        }

        /// <summary>
        /// Respawns a player after the specified delay, announcing the remaining seconds
        /// in the user's chat each second.
        /// </summary>
        /// <param name="user">The user to respawn.</param>
        /// <param name="pos">The position to respawn at.</param>
        /// <param name="delay">The delay in milliseconds before respawning.</param>
        public static void Respawn(IUser user, Vector2 pos, uint delay)
        {
            float endTime = Game.TotalElapsedGameTime + delay;

            Events.UpdateCallback respawnUpdate = Game.Events.StartUpdateCallback(_ =>
            {
                int secondsLeft = (int)MathF.Ceiling((endTime - Game.TotalElapsedGameTime) / 1000f);

                Game.ShowChatMessage($"You will spawn in {secondsLeft} seconds...",
                    _respawnColor, user.UserIdentifier);
            }, 1000);

            respawnUpdate.Invoke(0);

            Game.Events.StartUpdateCallback(dlt =>
            {
                respawnUpdate.Stop();
                respawnUpdate = null;

                Spawn(user, pos);
            }, delay, 1);
        }
    }
}
