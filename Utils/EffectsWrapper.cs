using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Typed helpers for playing effects, wrapping <see cref="IGame.PlayEffect"/>.
    /// </summary>
    public static class EffectsWrapper
    {
        /// <summary>
        /// Plays the <see cref="EffectName.CustomFloatText"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="text">Text to display.</param>
        public static void PlayCustomFloatText(Vector2 pos, string text) =>
            Game.PlayEffect(EffectName.CustomFloatText, pos, text);

        /// <summary>
        /// Plays the <see cref="EffectName.CustomFloatText"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="text">Text to display.</param>
        /// <param name="color">Color of the text.</param>
        public static void PlayCustomFloatText(Vector2 pos, string text, Color color) =>
            Game.PlayEffect(EffectName.CustomFloatText, pos, text, color);

        /// <summary>
        /// Plays the <see cref="EffectName.CustomFloatText"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="text">Text to display.</param>
        /// <param name="color">Color of the text.</param>
        /// <param name="duration">How long the text stays visible.</param>
        public static void PlayCustomFloatText(Vector2 pos, string text, Color color, float duration) =>
            Game.PlayEffect(EffectName.CustomFloatText, pos, text, color, duration);

        /// <summary>
        /// Plays the <see cref="EffectName.CustomFloatText"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="text">Text to display.</param>
        /// <param name="color">Color of the text.</param>
        /// <param name="duration">How long the text stays visible.</param>
        /// <param name="scale">Scale of the text.</param>
        public static void PlayCustomFloatText(Vector2 pos, string text, Color color, float duration, float scale) =>
            Game.PlayEffect(EffectName.CustomFloatText, pos, text, color, duration, scale);

        /// <summary>
        /// Plays the <see cref="EffectName.CustomFloatText"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="text">Text to display.</param>
        /// <param name="color">Color of the text.</param>
        /// <param name="duration">How long the text stays visible.</param>
        /// <param name="scale">Scale of the text.</param>
        /// <param name="outline">Whether the text is rendered with an outline.</param>
        public static void PlayCustomFloatText(Vector2 pos, string text, Color color, float duration, float scale, bool outline) =>
            Game.PlayEffect(EffectName.CustomFloatText, pos, text, color, duration, scale, outline);

        /// <summary>
        /// Plays the <see cref="EffectName.BulletSlowmoTrace"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="dirX">Direction X component.</param>
        /// <param name="dirY">Direction Y component.</param>
        public static void PlayBulletSlowmoTrace(Vector2 pos, float dirX, float dirY) =>
            Game.PlayEffect(EffectName.BulletSlowmoTrace, pos, dirX, dirY);

        /// <summary>
        /// Plays the <see cref="EffectName.BulletSlowmoTrace"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="dir">Direction of the trace.</param>
        public static void PlayBulletSlowmoTrace(Vector2 pos, Vector2 dir) =>
            Game.PlayEffect(EffectName.BulletSlowmoTrace, pos, dir.X, dir.Y);

        /// <summary>
        /// Plays the <see cref="EffectName.CameraShaker"/> effect at the given position.
        /// The shake is always applied globally.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="intensity">Intensity of the camera shake.</param>
        /// <param name="time">Duration of the camera shake.</param>
        public static void PlayCameraShaker(Vector2 pos, float intensity, float time) =>
            Game.PlayEffect(EffectName.CameraShaker, pos, intensity, time, true);

        /// <summary>
        /// Plays the <see cref="EffectName.Steam"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="steamColor">Color of the steam.</param>
        public static void PlaySteam(Vector2 pos, Color steamColor) =>
            Game.PlayEffect(EffectName.Steam, pos, steamColor);

        /// <summary>
        /// Plays the <see cref="EffectName.Steam"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="steamColor">Color of the steam.</param>
        /// <param name="steamScale">Scale of the steam.</param>
        public static void PlaySteam(Vector2 pos, Color steamColor, float steamScale) =>
            Game.PlayEffect(EffectName.Steam, pos, steamColor, steamScale);

        /// <summary>
        /// Plays the <see cref="EffectName.Steam"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="steamColor">Color of the steam.</param>
        /// <param name="steamScale">Scale of the steam.</param>
        /// <param name="fadeSpeed">Fade speed multiplier.</param>
        public static void PlaySteam(Vector2 pos, Color steamColor, float steamScale, float fadeSpeed) =>
            Game.PlayEffect(EffectName.Steam, pos, steamColor, steamScale, fadeSpeed);

        /// <summary>
        /// Plays the <see cref="EffectName.TraceSpawner"/> effect.
        /// Shows effects on a moving target with an even distribution depending on how
        /// fast the target is moving.
        /// </summary>
        /// <param name="target">Object to track.</param>
        /// <param name="effectID">Effect to keep spawning on the object.</param>
        /// <param name="spawnTime">How long the trace spawner stays alive.</param>
        public static void PlayTraceSpawner(IObject target, string effectID, float spawnTime) =>
            Game.PlayEffect(EffectName.TraceSpawner, Vector2.Zero, target.UniqueID, effectID, spawnTime);

        /// <summary>
        /// Plays the <see cref="EffectName.TraceSpawner"/> effect.
        /// Shows effects on a moving target with an even distribution depending on how
        /// fast the target is moving.
        /// </summary>
        /// <param name="uniqueID">Unique ID of the object to track.</param>
        /// <param name="effectID">Effect to keep spawning on the object.</param>
        /// <param name="spawnTime">How long the trace spawner stays alive.</param>
        public static void PlayTraceSpawner(int uniqueID, string effectID, float spawnTime) =>
            Game.PlayEffect(EffectName.TraceSpawner, Vector2.Zero, uniqueID, effectID, spawnTime);

        /// <summary>
        /// Plays the <see cref="EffectNamesExtra.MuzzleFlash"/> effect at the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="objectId">Object ID (any object: tile, player, etc.).</param>
        /// <param name="muzzleFlashType">Muzzle flash type. Accepted values from <see cref="EffectNamesExtra.MuzzleFlashTypes"/>.</param>
        public static void PlayMuzzleFlash(Vector2 pos, int objectId, string muzzleFlashType) =>
            Game.PlayEffect(EffectNamesExtra.MuzzleFlash, pos, objectId, muzzleFlashType);

        /// <summary>
        /// Plays the <see cref="EffectNamesExtra.OutOfAmmoRecoil"/> effect, showing the
        /// ranged-weapon out-of-ammo recoil animation on a player.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="playerId">Unique ID of the player to show the recoil on.</param>
        public static void PlayOutOfAmmoRecoil(Vector2 pos, int playerId) =>
            Game.PlayEffect(EffectNamesExtra.OutOfAmmoRecoil, pos, playerId);

        /// <summary>
        /// Plays the <see cref="EffectNamesExtra.PickupText"/> effect, showing a weapon
        /// pickup text floating above a player.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="weaponId">Weapon identifier, optionally with an ammo suffix
        /// (e.g. <c>"AssaultRifle"</c> or <c>"AssaultRifle_30"</c>).</param>
        public static void PlayPickupText(Vector2 pos, string weaponId) =>
            Game.PlayEffect(EffectNamesExtra.PickupText, pos, weaponId);

        /// <summary>
        /// Plays the <see cref="EffectNamesExtra.FireNodeFlamethrowerStart"/> effect,
        /// starting a FireNode flamethrower effect from the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="dirX">Direction X component.</param>
        /// <param name="dirY">Direction Y component.</param>
        public static void PlayFireNodeFlamethrowerStart(Vector2 pos, float dirX, float dirY) =>
            Game.PlayEffect(EffectNamesExtra.FireNodeFlamethrowerStart, pos, dirX, dirY);

        /// <summary>
        /// Plays the <see cref="EffectNamesExtra.FireNodeFlamethrowerStart"/> effect,
        /// starting a FireNode flamethrower effect from the given position.
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="dir">Direction of the flamethrower.</param>
        public static void PlayFireNodeFlamethrowerStart(Vector2 pos, Vector2 dir) =>
            Game.PlayEffect(EffectNamesExtra.FireNodeFlamethrowerStart, pos, dir.X, dir.Y);

        /// <summary>
        /// Plays the <see cref="EffectNamesExtra.FireListener"/> effect on an object.
        /// (Purpose unconfirmed.)
        /// </summary>
        /// <param name="pos">World position of the effect.</param>
        /// <param name="objectId">Object ID to attach the fire listener to.</param>
        public static void PlayFireListener(Vector2 pos, int objectId) =>
            Game.PlayEffect(EffectNamesExtra.FireListener, pos, objectId);
    }
}
