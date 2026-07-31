using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Constants for undocumented <see cref="Game.PlayEffect"/> effect names,
    /// following the same pattern as <see cref="EffectName"/>.
    /// </summary>
    public static class EffectNamesExtra
    {
        /// <summary>
        /// Muzzle flash effect on a tile.
        /// <code>Game.PlayEffect("MZLED", Vector2.Zero, tileUniqueId, muzzleFlashType)</code>
        /// </summary>
        /// <remarks>
        /// <b>param1</b> (<see cref="int"/>) — UniqueID of the tile holding the muzzle;
        /// the tile's angle is used to rotate the muzzle flash.<br/>
        /// <b>param2</b> (<see cref="string"/>) — Muzzle flash type. Accepted values:
        /// • MuzzleFlashS
        /// • MuzzleFlashM
        /// • MuzzleFlashL
        /// • MuzzleFlashBazooka
        /// • MuzzleFlashAssaultRifle
        /// • MuzzleFlashShotgun
        /// </remarks>
        public const string MuzzleFlash = "MZLED";

        /// <summary>
        /// Shows the ranged-weapon out-of-ammo recoil animation on a player.
        /// <code>Game.PlayEffect("OOAC", Vector2.Zero, playerId)</code>
        /// </summary>
        /// <remarks>
        /// <b>param1</b> (<see cref="int"/>) — <see cref="IPlayer.UniqueID"/> of the player
        /// to show the recoil on.
        /// </remarks>
        public const string OutOfAmmoRecoil = "OOAC";

        /// <summary>
        /// Shows a weapon pickup text floating above a player.
        /// <code>Game.PlayEffect("PWT", Vector2.Zero, weaponId)</code>
        /// </summary>
        /// <remarks>
        /// <b>param1</b> (<see cref="string"/>) — Weapon identifier.
        /// Plain weapon name (e.g. <c>"AssaultRifle"</c>) for a normal pickup,
        /// or weapon name with ammo suffix (e.g. <c>"AssaultRifle_30"</c>) for a
        /// flagged pickup with ammo count.
        /// </remarks>
        public const string PickupText = "PWT";

        /// <summary>
        /// Spawns a FireNode flamethrower effect at the given world coordinates.
        /// <code>Game.PlayEffect("FNFTST", Vector2.Zero, x, y)</code>
        /// </summary>
        /// <remarks>
        /// <b>param1</b> (<see cref="float"/>) — X coordinate of the effect origin.<br/>
        /// <b>param2</b> (<see cref="float"/>) — Y coordinate of the effect origin.
        /// </remarks>
        public const string FireNodeFlamethrower = "FNFTST";

        /// <summary>
        /// Fire listener effect on an object. (Purpose unconfirmed.)
        /// <code>Game.PlayEffect("FLST", Vector2.Zero, objectId)</code>
        /// </summary>
        /// <remarks>
        /// <b>param1</b> (<see cref="int"/>) — ObjectID to attach the fire listener to.
        /// </remarks>
        public const string FireListener = "FLST";

        /// <summary>
        /// Fire node spawner effect. Animates the appearance of a FireNode at
        /// the given position.
        /// <code>Game.PlayEffect("FND", position)</code>
        /// </summary>
        public const string FireNodeSpawner = "FND";

        /// <summary>
        /// Fire big effect on a player. (Purpose unconfirmed.)
        /// <code>Game.PlayEffect("FBG", Vector2.Zero, playerId)</code>
        /// </summary>
        /// <remarks>
        /// <b>param1</b> (<see cref="int"/>) — <see cref="IPlayer.UniqueID"/> of the player
        /// to apply the effect to.
        /// </remarks>
        public const string FireBig = "FBG";
    }
}