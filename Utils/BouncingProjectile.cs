using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// A <see cref="CustomProjectile"/> that bounces off non-destructable surfaces
    /// instead of being disabled, up to a configurable number of times.
    /// </summary>
    public class BouncingProjectile : CustomProjectile
    {
        /// <summary>
        /// Remaining non-destructable bounces before the projectile stops.
        /// Decreases by one on each bounce. Defaults to 2.
        /// </summary>
        public int Bounces = 2;

        /// <summary>
        /// Delegate for handling when the projectile bounces off a non-destructable surface.
        /// </summary>
        /// <param name="result">The ray-cast hit against the non-destructable surface.</param>
        /// <param name="proj">The projectile that bounced.</param>
        public delegate void OnBounceCallback(RayCastResult result, BouncingProjectile proj);
        public OnBounceCallback OnBounce;

        /// <summary>
        /// Creates a new bouncing projectile with the specified position, direction and
        /// ray-cast collision settings. <see cref="CustomProjectile.Speed"/> defaults to 1.
        /// </summary>
        public BouncingProjectile(Vector2 pos, Vector2 direction, RayCastInput rayCastCollision) : base(pos, direction, rayCastCollision) { }

        /// <summary>
        /// Creates a new bouncing projectile with the specified position, direction,
        /// initial speed and ray-cast collision settings.
        /// </summary>
        public BouncingProjectile(Vector2 pos, Vector2 direction, float speed, RayCastInput rayCastCollision) : base(pos, direction, rayCastCollision)
        {
            Speed = speed;
        }

        /// <summary>
        /// Creates a new bouncing projectile that inherits its base behavior from an
        /// existing <see cref="CustomProjectile"/>. <see cref="Bounces"/> defaults to 2.
        /// </summary>
        public BouncingProjectile(Vector2 pos, Vector2 direction, CustomProjectile proj) : base(pos, direction, proj) { }

        /// <summary>
        /// Creates a new bouncing projectile that inherits both its base behavior and
        /// bounce configuration from another <see cref="BouncingProjectile"/>.
        /// </summary>
        public BouncingProjectile(Vector2 pos, Vector2 direction, BouncingProjectile proj) : base(pos, direction, proj)
        {
            Bounces = proj.Bounces;
        }

        /// <summary>
        /// Bounces the projectile off a non-destructable surface by reflecting its
        /// <see cref="CustomProjectile.Direction"/> across the hit normal, consuming
        /// one <see cref="Bounces"/>. Returns <c>false</c> to keep the projectile flying.
        /// When <see cref="Bounces"/> reaches 0 the projectile is allowed to stop
        /// (the default base behavior).
        /// </summary>
        /// <param name="result">The ray-cast hit against the non-destructable surface.</param>
        protected override bool OnNonDestructableHit(RayCastResult result)
        {
            if (Bounces == 0)
                return true;

            Bounces--;
            Position = result.Position;
            Direction = Vector2Helper.Bounce(Direction, result.Normal);
            OnBounce?.Invoke(result, this);
            return false;
        }
    }
}