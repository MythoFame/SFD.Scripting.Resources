using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// General-purpose utilities for <see cref="IProjectile"/>.
    /// </summary>
    public static class ProjectileHelper
    {
        /// <summary>
        /// Gets the currently active powerup for a projectile.
        /// </summary>
        /// <param name="proj">The projectile to get the powerup of.</param>
        /// <returns>ProjectilePowerup enum indicating the active powerup.</returns>
        public static ProjectilePowerup GetPowerup(IProjectile proj) => proj switch
        {
            { PowerupBounceActive: true } => ProjectilePowerup.Bouncing,
            { PowerupFireActive: true } => ProjectilePowerup.Fire,
            _ => ProjectilePowerup.None
        };
    }
}
