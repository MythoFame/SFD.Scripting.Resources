using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// A <see cref="CustomProjectile"/> that mimics the physics of SFD's FireNodes:
    /// travels forward, is pulled down by gravity, decelerates via friction, and
    /// can settle on the ground when it hits a non-destructible surface. While
    /// airborne it casts swept ray-casts along its traveled path (like the base
    /// class), so it can hit players reliably in flight; once settled it senses
    /// targets with a vertical raycast segment, see <see cref="Lingering"/>.
    /// </summary>
    /// <remarks>
    /// Non-destructible objects are treated as the resting surface rather than valid
    /// targets: by default they still "land" (decrement <see cref="CustomProjectile.PiercingTargets"/>),
    /// so with the default <c>PiercingTargets = 1</c> the projectile will disable on the
    /// first wall hit instead of lingering. To make <see cref="Lingering"/> effective,
    /// either raise <see cref="CustomProjectile.PiercingTargets"/> or return
    /// <c>false</c> from <see cref="CustomProjectile.OnHit"/> for non-destructible
    /// objects (check <c>!hitObject.Destructable</c>).
    /// </remarks>
    public class NodeProjectile : CustomProjectile
    {
        private bool _isStatic = false;
        private float _elapsed = 0;
        private float _raycastCooldown = 0;

        private const float RAYCAST_COOLDOWN_MS = 50;

        /// <summary>
        /// Downward acceleration applied to the projectile's velocity each update,
        /// in units/ms². Defaults to -0.008.
        public float Gravity = -0.008f;

        /// <summary>
        /// Deceleration applied to the velocity magnitude each update, in units/ms.
        /// Applied before gravity so the downward pull is never eroded. Defaults to 0.01.
        /// </summary>
        public float Friction = 0.01f;

        /// <summary>
        /// How long the projectile lives before being disabled, in milliseconds.
        /// Defaults to 5000.
        /// </summary>
        public float Lifetime = 5000f;

        /// <summary>
        /// Whether the projectile may settle in place when it hits a non-destructible
        /// surface (instead of being disabled). Once settled, <see cref="IsStatic"/>
        /// becomes true and the projectile keeps sensing targets once every 50ms
        /// like a proximity mine until <see cref="Lifetime"/> expires or
        /// <see cref="CustomProjectile.PiercingTargets"/> runs out.
        /// </summary>
        public bool Lingering = false;

        /// <summary>
        /// Length of the vertical raycast segment used for collision detection once
        /// the projectile is static, centered on <see cref="CustomProjectile.Position"/>.
        /// Defaults to 16.
        /// </summary>
        public float RayCastLength = 16;

        /// <summary>
        /// Whether the projectile has settled on a surface and is no longer moving.
        /// While static, physics updates are skipped and collision sensing runs once
        /// every 50ms (so callbacks can assume a fixed ~50ms interval) instead of
        /// every frame.
        /// </summary>
        public bool IsStatic => _isStatic;

        /// <summary>
        /// Creates a new node projectile with the specified position, direction and
        /// ray cast collision settings. <see cref="CustomProjectile.Speed"/> defaults
        /// to 1; set it (or use the <see cref="NodeProjectile(Vector2, Vector2, float, RayCastInput)"/>
        /// constructor) for a faster launch.
        /// </summary>
        public NodeProjectile(Vector2 pos, Vector2 direction, RayCastInput rayCastCollision) : base(pos, direction, rayCastCollision) { }

        /// <summary>
        /// Creates a new node projectile with the specified position, direction,
        /// initial speed and ray cast collision settings.
        /// </summary>
        public NodeProjectile(Vector2 pos, Vector2 direction, float speed, RayCastInput rayCastCollision) : base(pos, direction, rayCastCollision)
        {
            Speed = speed;
        }

        /// <summary>
        /// Creates a new node projectile that inherits its base behavior from an
        /// existing <see cref="CustomProjectile"/>. Node-specific fields
        /// (<see cref="Gravity"/>, <see cref="Friction"/>, <see cref="Lifetime"/>,
        /// <see cref="Lingering"/>, <see cref="RayCastLength"/>) take their defaults.
        /// </summary>
        public NodeProjectile(Vector2 pos, Vector2 direction, CustomProjectile proj) : base(pos, direction, proj) { }

        /// <summary>
        /// Creates a new node projectile that inherits both its base behavior and
        /// node-specific configuration from another <see cref="NodeProjectile"/>.
        /// </summary>
        public NodeProjectile(Vector2 pos, Vector2 direction, NodeProjectile proj) : base(pos, direction, proj)
        {
            Gravity = proj.Gravity;
            Friction = proj.Friction;
            Lifetime = proj.Lifetime;
            Lingering = proj.Lingering;
            RayCastLength = proj.RayCastLength;
        }

        protected override void Update(float dlt)
        {
            _elapsed += dlt;
            if (_elapsed >= Lifetime)
            {
                Enabled = false;
                return;
            }

            Vector2 half = Vector2.UnitY * (RayCastLength / 2);

            if (!_isStatic)
            {
                Vector2 vel = Velocity;

                float speed = vel.Length();
                if (speed > 0)
                {
                    float newSpeed = Math.Max(0, speed - Friction * dlt);
                    vel *= newSpeed / speed;
                }

                vel.Y += Gravity * dlt;

                if (vel != Vector2.Zero)
                    Velocity = vel;

                _position += vel * dlt;

                Game.DrawLine(_subPosition, Position, Color.Yellow);

                Vector2 rayCastEnd = _position;

                if (Vector2.Distance(_subPosition, _position) < MIN_RAYCAST_LENGTH)
                    rayCastEnd = _subPosition + Direction * MIN_RAYCAST_LENGTH;

                RayCastResult[] results = Game.RayCast(_subPosition, rayCastEnd, RayCastCollision);

                foreach (RayCastResult result in results)
                {
                    if (!result.Hit) continue;

                    bool destructable = result.IsPlayer || result.HitObject.Destructable;
                    bool landed = OnHit?.Invoke(result, this) ?? true;

                    if (landed && PiercingTargets > 0)
                        PiercingTargets--;

                    if (PiercingTargets == 0)
                    {
                        Enabled = false;
                        break;
                    }

                    if (!destructable)
                    {
                        if (Wallbang)
                            continue;

                        if (Lingering)
                        {
                            Position = result.Position;
                            Velocity = Vector2.Zero;
                            _isStatic = true;
                            continue;
                        }

                        Enabled = false;
                        break;
                    }
                }

                _subPosition = _position;
            }
            else
            {
                _raycastCooldown += dlt;
                if (_raycastCooldown >= RAYCAST_COOLDOWN_MS)
                {
                    _raycastCooldown = 0;

                    Vector2 bottom = Position - half;
                    Vector2 top = Position + half;

                    Game.DrawLine(bottom, top);

                    RayCastResult[] results = Game.RayCast(bottom, top, RayCastCollision);

                    foreach (RayCastResult result in results)
                    {
                        if (!result.Hit) continue;

                        bool destructable = result.IsPlayer || result.HitObject.Destructable;
                        bool landed = OnHit?.Invoke(result, this) ?? true;

                        if (landed && PiercingTargets > 0)
                            PiercingTargets--;

                        if (PiercingTargets == 0)
                        {
                            Enabled = false;
                            break;
                        }

                        if (!destructable)
                            continue;
                    }
                }
            }

            Game.PlayEffect(Effect, Position);
        }
    }
}
