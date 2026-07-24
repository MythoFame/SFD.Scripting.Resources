using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// A <see cref="CustomProjectile"/> that steers itself towards a target during flight,
    /// rather than travelling in a straight line. Target selection and steering strength
    /// are configurable; the default behavior homes in on the closest enemy player.
    /// </summary>
    public class HomingProjectile : CustomProjectile
    {
        private float _homing = 1;

        /// <summary>
        /// The player that fired the projectile. Used to exclude the shooter from
        /// potential targets and to determine the projectile's <see cref="Team"/>.
        /// </summary>
        public IPlayer Shooter = null;

        /// <summary>
        /// How strongly the projectile steers towards its target each update, clamped
        /// between 0 (no homing) and 1 (instant snap to target direction).
        /// </summary>
        public float Homing
        {
            get => _homing; set => _homing = MathHelper.Clamp(value, 0, 1);
        }

        /// <summary>
        /// The team the projectile belongs to, derived from <see cref="Shooter"/>.
        /// Setting this updates the shooter's team. When <see cref="Shooter"/> is null,
        /// the team is <see cref="PlayerTeam.Independent"/>.
        /// </summary>
        public PlayerTeam Team
        {
            get => Shooter != null ? Shooter.GetTeam() : PlayerTeam.Independent; set => Shooter.SetTeam(value);
        }

        /// <summary>
        /// The nearest living player considered a valid homing target. A player is
        /// eligible when it is not on the projectile's team (or is independent) and
        /// is not the <see cref="Shooter"/>. Returns null when no such player exists.
        /// </summary>
        protected IPlayer ClosestEnemy
        {
            get
            {
                List<IPlayer> enemies =
                [.. Game.GetPlayers()
                        .Where(p => (p.GetTeam() != Team ||
                            p.GetTeam() == PlayerTeam.Independent) &&
                          !p.IsDead && p != Shooter)];

                enemies.Sort((p1, p2) =>
                  Vector2.Distance(p1.GetWorldPosition(), Position)
                  .CompareTo(Vector2.Distance(p2.GetWorldPosition(),
                    Position)));

                return enemies.FirstOrDefault();
            }
        }

        /// <summary>
        /// Creates a new homing projectile with the specified position, direction and ray cast
        /// collision settings. <see cref="Shooter"/> and <see cref="Homing"/> default to null and 1.
        /// </summary>
        public HomingProjectile(Vector2 pos, Vector2 direction, RayCastInput rayCastCollision) : base(pos, direction, rayCastCollision)
        {
        }

        /// <summary>
        /// Creates a new homing projectile that inherits its behavior from an existing
        /// <see cref="CustomProjectile"/>. <see cref="Shooter"/> and <see cref="Homing"/> still
        /// default to null and 1 and must be set separately.
        /// </summary>
        public HomingProjectile(Vector2 pos, Vector2 direction, CustomProjectile proj) : base(pos, direction, proj)
        {
        }

        /// <summary>
        /// Creates a new homing projectile that inherits both its base behavior and homing
        /// configuration (including <see cref="Homing"/> and <see cref="Shooter"/>) from another
        /// <see cref="HomingProjectile"/>.
        /// </summary>
        public HomingProjectile(Vector2 pos, Vector2 direction, HomingProjectile proj) : base(pos, direction, proj)
        {
            Homing = proj.Homing;
            Shooter = proj.Shooter;
        }

        /// <summary>
        /// Determines the world position the projectile should home towards.
        /// By default, targets the closest enemy player. Override to implement
        /// custom homing behavior (e.g. homing towards a fixed point, an object,
        /// or a different target-selection algorithm).
        /// </summary>
        /// <returns>The position to home towards, or null if there is no valid target.</returns>
        protected virtual Vector2? GetHomingTargetPosition()
        {
            IPlayer closestEnemy = ClosestEnemy;

            if (closestEnemy == null)
                return null;

            return closestEnemy.GetAABB().Center;
        }

        /// <summary>
        /// Updates the projectile's position via the base implementation, then steers its
        /// <see cref="CustomProjectile.Direction"/> towards the position returned by
        /// <see cref="GetHomingTargetPosition"/> using <see cref="Homing"/> as the step size.
        /// </summary>
        protected override void Update(float dlt)
        {
            base.Update(dlt);

            Vector2? targetPosition = GetHomingTargetPosition();

            if (targetPosition == null)
                return;

            // Calculate the target direction to the target position
            Vector2 targetDirection = Vector2Helper.DirectionTo(Position, targetPosition.Value);

            // Smoothly interpolate the current direction towards the target direction
            Direction = Vector2Helper.MoveToward(Direction, targetDirection, Homing);
        }
    }
}
