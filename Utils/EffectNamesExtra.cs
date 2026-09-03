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
        /// <code>Game.PlayEffect("MZLED", Vector2.Zero, objectId, muzzleFlashType)</code>
        /// </summary>
        /// <remarks>
        /// <b>param1</b> (<see cref="int"/>) — Object ID (any object: tile, player, etc.).<br/>
        /// <b>param2</b> (<see cref="string"/>) — Muzzle flash type. Accepted values from <see cref="MuzzleFlashTypes"/>.
        /// </remarks>
        public const string MuzzleFlash = "MZLED";

        /// <summary>
        /// Muzzle-flash type identifiers used with <see cref="MuzzleFlash"/>.
        /// </summary>
        public static class MuzzleFlashTypes
        {
            public const string S = "MuzzleFlashS";
            public const string M = "MuzzleFlashM";
            public const string L = "MuzzleFlashL";
            public const string Bazooka = "MuzzleFlashBazooka";
            public const string AssaultRifle = "MuzzleFlashAssaultRifle";
            public const string Shotgun = "MuzzleFlashShotgun";
        }

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
        /// Starts a FireNode flamethrower effect. Direction is passed via args; the effect origin is the <code>worldPosition</code> given to <see cref="IGame.PlayEffect"/>.
        /// <code>Game.PlayEffect("FNFTST", Vector2.Zero, dirX, dirY)</code>
        /// </summary>
        /// <remarks>
        /// <b>param1</b> (<see cref="float"/>) — Direction X component (<c>dirX</c>).<br/>
        /// <b>param2</b> (<see cref="float"/>) — Direction Y component (<c>dirY</c>).
        /// </remarks>
        public const string FireNodeFlamethrowerStart = "FNFTST";

        /// <summary>
        /// Fire listener effect on an object. (Purpose unconfirmed.)
        /// <code>Game.PlayEffect("FLST", Vector2.Zero, objectId)</code>
        /// </summary>
        /// <remarks>
        /// <b>param1</b> (<see cref="int"/>) — ObjectID to attach the fire listener to.
        /// </remarks>
        public const string FireListener = "FLST";

        // Non-functional through PlayEffect — kept for reference.
        /// <summary>
        /// Fire node effect (internal name "FireNode"). Non-functional via <see cref="IGame.PlayEffect"/>.
        /// <code>Game.PlayEffect("FND", position)</code>
        /// </summary>
        // public const string FireNode = "FND";

        // Non-functional through PlayEffect — kept for reference.
        /// <summary>
        /// Fire big effect (internal name "FireBig"). Non-functional via <see cref="IGame.PlayEffect"/>.
        /// <code>Game.PlayEffect("FBG", Vector2.Zero, objectId)</code>
        /// </summary>
        // public const string FireBig = "FBG";
    }
}