using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    public static class PlayerHelper
    {
        private static readonly Vector2 _stickyFeetTransition = new(0, 2);

        public static void Unstick(IPlayer player) => player.SetWorldPosition(player.GetWorldPosition() + _stickyFeetTransition);

        public static bool IsManualFiring(IPlayer player)
        {
            if (!player.IsManualAiming || !player.KeyPressed(VirtualKey.ATTACK))
                return false;

            return player.CurrentWeaponDrawn switch
            {
                WeaponItemType.Handgun => player.CurrentSecondaryRangedWeapon.TotalAmmo > 0,
                WeaponItemType.Rifle => player.CurrentPrimaryRangedWeapon.TotalAmmo > 0,
                _ => false
            };
        }

        public static bool IsFiring(IPlayer player) => player.IsHipFiring || IsManualFiring(player);
    }
}
