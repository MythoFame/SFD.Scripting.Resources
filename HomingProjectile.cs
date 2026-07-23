using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    public class HomingProjectile : CustomProjectile
    {
        private float _homing = 1;

        public IPlayer Shooter = null;

        public float Homing
        {
            get => _homing; set => _homing = MathHelper.Clamp(value, 0, 1);
        }

        public PlayerTeam Team
        {
            get => Shooter != null ? Shooter.GetTeam() : PlayerTeam.Independent; set => Shooter.SetTeam(value);
        }

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

        public HomingProjectile(Vector2 pos, Vector2 direction, RayCastInput rayCastCollision) : base(pos, direction, rayCastCollision)
        {
        }

        public HomingProjectile(Vector2 pos, Vector2 direction, CustomProjectile proj) : base(pos, direction, proj)
        {
        }

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
