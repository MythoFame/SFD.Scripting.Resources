using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Returns a random <see cref="WeaponItem"/> matching the given <see cref="WeaponItemType"/>.
    /// Draws random weapons via <see cref="IGame.GetRandomWeaponItem"/> and spawns them
    /// transiently to inspect their type, retrying until a match is found.
    /// </summary>
    /// <param name="type">The desired weapon category (e.g. handgun, rifle, melee).</param>
    /// <returns>A <see cref="WeaponItem"/> whose <see cref="WeaponItemType"/> equals <paramref name="type"/>.</returns>
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
}
