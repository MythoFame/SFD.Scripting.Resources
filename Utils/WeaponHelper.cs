using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Provides helper methods for working with <see cref="WeaponItem"/> and <see cref="WeaponItemType"/>.
    /// </summary>
    public static class WeaponHelper
    {
        /// <summary>
        /// Returns a random <see cref="WeaponItem"/> matching the given <see cref="WeaponItemType"/>.
        /// Draws random weapons via <see cref="IGame.GetRandomWeaponItem"/> and spawns them
        /// transiently to inspect their type, retrying until a match is found.
        /// </summary>
        /// <param name="type">The desired weapon category (e.g. handgun, rifle, melee).</param>
        /// <returns>A <see cref="WeaponItem"/> whose <see cref="WeaponItemType"/> equals <paramref name="type"/>.</returns>
        /// <remarks>
        /// Drawn weapons never include <see cref="WeaponItem.STREETSWEEPER"/> (see
        /// <see cref="RandomWeaponItemSafe"/>). Each inspected weapon is spawned with a
        /// near-zero despawn time, and the matching item is removed before returning.
        /// The method retries indefinitely when no drawn weapon ever matches
        /// <paramref name="type"/>.
        /// </remarks>
        public static WeaponItem GetRandomWeaponFromType(WeaponItemType type)
        {
            WeaponItem w = RandomWeaponItemSafe;

            IObjectWeaponItem wItem = Game.SpawnWeaponItem(w,
              Vector2.Zero, false, float.Epsilon);

            while (wItem.WeaponItemType != type)
            {
                w = RandomWeaponItemSafe;

                wItem = Game.SpawnWeaponItem(w,
                  Vector2.Zero, false, float.Epsilon);
            }

            wItem?.Remove();

            return w;
        }

        /// <summary>
        /// Draws a random weapon via <see cref="IGame.GetRandomWeaponItem"/>, retrying
        /// while the result is <see cref="WeaponItem.STREETSWEEPER"/>, which is excluded
        /// from the pool.
        /// </summary>
        private static WeaponItem RandomWeaponItemSafe
        {
            get
            {
                WeaponItem w = Game.GetRandomWeaponItem();

                while (w == WeaponItem.STREETSWEEPER)
                    w = Game.GetRandomWeaponItem();

                return w;
            }
        }

        /// <summary>
        /// Returns the default ammo for the specified weapon.
        /// Spawns the weapon transiently to query its ammo value, then removes it.
        /// </summary>
        /// <param name="weaponItem">The weapon to query.</param>
        /// <returns>The default ammo amount.</returns>
        public static float GetDefaultAmmo(WeaponItem weaponItem)
        {
            IObjectWeaponItem wItem = Game.SpawnWeaponItem(weaponItem,
              Vector2.Zero, false, float.Epsilon);

            float defaultAmmo = wItem.GetCurrentAmmo();

            wItem?.Remove();

            return defaultAmmo;
        }
    }
}
