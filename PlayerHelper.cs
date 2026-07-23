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
    }
}
