using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Represents a custom projectile with customizable behavior and collision handling.
    /// </summary>
    public class CustomProjectile
    {
        private const uint COOLDOWN = 0;
        private const float MIN_RAYCAST_LENGTH = 4;

        private Vector2 _direction;
        private Vector2 _position;
        private Vector2 _subPosition;

        private RayCastInput _rayCastCollision;

        private Events.UpdateCallback _updateCallback = null;

        private float _distanceTravelled = 0;

        /// <summary>
        /// The amount of remaining targets this projectile can hit before being disabled.
        /// Decreases by one each time a hit lands. Defaults to 1 (i.e. disables after landing a single hit).
        /// </summary>
        public ushort PiercingTargets = 1;

        /// <summary>
        /// Speed of the projectile.
        /// </summary>
        public float Speed = 1;

        /// <summary>
        /// The maximum distance the projectile can travel before being disabled.
        /// </summary>
        public float MaxDistanceTravelled = 1000;

        /// <summary>
        /// Effect to be played on movement.
        /// </summary>
        public string Effect = string.Empty;

        /// <summary>
        /// Whether the projectile ignores non-destructable objects (e.g. walls),
        /// allowing it to pass through them instead of being disabled on impact.
        /// </summary>
        public bool Wallbang = false;

        /// <summary>
        /// Information about the collision to be used for ray casting.
        /// </summary>
        public RayCastInput RayCastCollision
        {
            get => _rayCastCollision;
            set
            {
                _rayCastCollision = value;
                _rayCastCollision.ClosestHitOnly = true;
            }
        }

        /// <summary>
        /// Current position of the projectile.
        /// </summary>
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                _subPosition = _position;
            }
        }

        /// <summary>
        /// Direction of the projectile.
        /// </summary>
        public Vector2 Direction
        {
            get => _direction; set => _direction = Vector2.Normalize(value);
        }

        /// <summary>
        /// Velocity of the projectile.
        /// </summary>
        public Vector2 Velocity
        {
            get => Direction * Speed;
            set
            {
                Speed = value.Length();
                Direction = value;
            }
        }

        /// <summary>
        /// Indicates whether the projectile is enabled or not.
        /// </summary>
        public bool Enabled
        {
            get => _updateCallback != null;
            set
            {
                if (value != Enabled)
                {
                    if (value)
                    {
                        _updateCallback = Game.Events.StartUpdateCallback(Update, COOLDOWN);
                    }
                    else
                    {
                        _updateCallback.Stop();
                        _updateCallback = null;
                    }
                }
            }
        }

        /// <summary>
        /// The distance that the projectile has travelled.
        /// </summary>
        public float DistanceTravelled
        {
            get => _distanceTravelled;
            set
            {
                _distanceTravelled = value;

                Enabled = value <= MaxDistanceTravelled;
            }
        }

        /// <summary>
        /// Delegate for handling when the projectile hits a player.
        /// </summary>
        /// <param name="hitPlayer">The player hit by the projectile.</param>
        /// <param name="hitPosition">The position at which the projectile hit.</param>
        /// <returns>Whether the shot landed. A landed hit decreases the remaining piercing targets.</returns>
        public delegate bool OnPlayerHitCallback(IPlayer hitPlayer, Vector2 hitPosition, CustomProjectile proj);
        public OnPlayerHitCallback OnPlayerHit;

        /// <summary>
        /// Delegate for handling when the projectile hits an object.
        /// </summary>
        /// <param name="hitObject">The object hit by the projectile.</param>
        /// <param name="hitPosition">The position at which the projectile hit.</param>
        /// <returns>Whether the shot landed. A landed hit decreases the remaining piercing targets.</returns>
        public delegate bool OnObjectHitCallback(IObject hitObject, Vector2 hitPosition, CustomProjectile proj);
        public OnObjectHitCallback OnObjectHit;

        /// <summary>
        /// Initializes a new instance of the CustomProjectile class.
        /// </summary>
        /// <param name="pos">Initial position of the projectile.</param>
        /// <param name="direction">Initial direction of the projectile.</param>
        /// <param name="rayCastCollision">Information about the collision to be used for ray casting.</param>
        public CustomProjectile(Vector2 pos, Vector2 direction, RayCastInput rayCastCollision)
        {
            Position = pos;
            Direction = direction;
            RayCastCollision = rayCastCollision;
            Enabled = true;
        }

        /// <summary>
        /// Initializes a new instance of the CustomProjectile class copying another.
        /// </summary>
        /// <param name="pos">Initial position of the projectile.</param>
        /// <param name="direction">Initial direction of the projectile.</param>
        /// <param name="proj">Projectile to copy.</param>
        public CustomProjectile(Vector2 pos, Vector2 direction, CustomProjectile proj)
        {
            Position = pos;
            Direction = direction;
            PiercingTargets = proj.PiercingTargets;
            Speed = proj.Speed;
            MaxDistanceTravelled = proj.MaxDistanceTravelled;
            Effect = proj.Effect;
            Wallbang = proj.Wallbang;
            RayCastCollision = proj.RayCastCollision;
            OnPlayerHit = proj.OnPlayerHit;
            OnObjectHit = proj.OnObjectHit;
            Enabled = true;
        }

        protected virtual void Update(float dlt)
        {
            Vector2 vel = Velocity * Game.SlowmotionModifier;

            DistanceTravelled += vel.Length();

            _position += vel;

            Game.DrawLine(_subPosition, Position, Color.Yellow);

            Vector2 rayCastEnd = Position;

            if (Vector2.Distance(_subPosition, Position) < MIN_RAYCAST_LENGTH)
                rayCastEnd = _subPosition + Direction * MIN_RAYCAST_LENGTH;

            RayCastResult checkedResult = Game.RayCast(_subPosition, rayCastEnd, RayCastCollision)[0];

            bool landed = false;

            if (checkedResult.Hit)
            {
                if (checkedResult.IsPlayer)
                {
                    IPlayer hitPlayer = (IPlayer)checkedResult.HitObject;

                    landed = OnPlayerHit?.Invoke(hitPlayer, checkedResult.Position, this) ?? true;
                }
                else
                {
                    landed = OnObjectHit?.Invoke(checkedResult.HitObject, checkedResult.Position, this) ?? true;
                }

                if (landed && PiercingTargets > 0)
                    PiercingTargets--;

                bool blockedByObject = !checkedResult.IsPlayer && !checkedResult.HitObject.Destructable && !Wallbang;

                Enabled = PiercingTargets > 0 && !blockedByObject;
            }

            Game.PlayEffect(Effect, _subPosition);

            Vector2 trailEnd = checkedResult.Hit && landed && !Enabled ? checkedResult.Position : Position;

            PointShape.Trail(Draw, _subPosition, trailEnd, 5);

            _subPosition += vel;
        }

        private void Draw(Vector2 pos) => Game.PlayEffect(Effect, pos);
    }
}
