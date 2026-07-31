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
        /// Gets whether a player currently has a ranged weapon, whether it be the
        /// primary or secondary weapon slot.
        /// </summary>
        public static bool HasRangedWeapon(IPlayer player) => player.CurrentPrimaryWeapon.WeaponItem != WeaponItem.NONE ||
          player.CurrentSecondaryWeapon.WeaponItem != WeaponItem.NONE;

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
    }
}
