using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Helpers for interacting with colors.
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// Returns a random <see cref="Color"/> with a fully random RGB value and default
        /// (opaque) alpha.
        /// </summary>
        /// <param name="random">A Random instance for generating the color.</param>
        public static Color GetRandomColor(Random random) =>
            new((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256));
    }
}
