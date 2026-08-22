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

        /// <summary>
        /// Returns the standard <see cref="Color"/> associated with a given team.
        /// </summary>
        /// <param name="team">The team for which to retrieve the color.</param>
        public static Color GetTeamColor(PlayerTeam team) => team switch
        {
            PlayerTeam.Team1 => Color.Blue,
            PlayerTeam.Team2 => Color.Red,
            PlayerTeam.Team3 => Color.Green,
            PlayerTeam.Team4 => Color.Yellow,
            PlayerTeam.Team5 => Color.Cyan,
            PlayerTeam.Team6 => Color.Magenta,
            PlayerTeam.Team7 => new(137, 64, 0),
            PlayerTeam.Team8 => new(156, 92, 71),
            _ => Color.White,
        };

        /// <summary>
        /// Returns the hexadecimal representation of a <see cref="Color"/> in the form
        /// <c>#RRGGBB</c>, optionally followed by the alpha component as <c>#RRGGBBAA</c>.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        /// <param name="includeAlpha">Whether to append the alpha component.</param>
        public static string ToHex(Color color, bool includeAlpha = false) =>
            includeAlpha
                ? $"#{color.R:X2}{color.G:X2}{color.B:X2}{color.A:X2}"
                : $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        /// <summary>
        /// Returns the <see cref="Color"/> represented by a hexadecimal string in the form
        /// <c>RRGGBB</c> or <c>RRGGBBAA</c>, with an optional leading <c>#</c>. A missing
        /// alpha component defaults to opaque and hex digits are case-insensitive.
        /// </summary>
        /// <param name="hex">The hexadecimal string to parse.</param>
        /// <exception cref="ArgumentNullException"><paramref name="hex"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="hex"/> is not 6 or 8 hexadecimal digits.</exception>
        public static Color FromHex(string hex)
        {
            ArgumentNullException.ThrowIfNull(hex);

            if (hex.Length > 0 && hex[0] == '#')
                hex = hex[1..];

            if (hex.Length != 6 && hex.Length != 8)
                throw new FormatException($"'{hex}' is not a valid hexadecimal color.");

            return new Color(
                Convert.ToByte(hex[..2], 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16),
                hex.Length == 8 ? Convert.ToByte(hex.Substring(6, 2), 16) : (byte)255
            );
        }

        /// <summary>
        /// Converts a named color string to a Color member.
        /// </summary>
        /// <param name="name">The named color string.</param>
        public static bool TryParseNamed(string name, out Color color)
        {
            switch (name)
            {
                case "White": color = Color.White; return true;
                case "Red": color = Color.Red; return true;
                case "Green": color = Color.Green; return true;
                case "Blue": color = Color.Blue; return true;
                case "Yellow": color = Color.Yellow; return true;
                case "Magenta": color = Color.Magenta; return true;
                case "Cyan": color = Color.Cyan; return true;
                case "Black": color = Color.Black; return true;
                case "Grey": color = Color.Grey; return true;
                case "Transparent": color = Color.Transparent; return true;
                default: color = default; return false;
            }
        }
    }
}
