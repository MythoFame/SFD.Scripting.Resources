using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Reusable string parsers for common SFD types, intended for chat commands.
    /// All methods return an <see cref="IEnumerable{T}"/> and yield an empty
    /// collection when nothing matches. Bidirectional and case-insensitive matching
    /// use the supplied <see cref="StringComparison"/> (default <see cref="StringComparison.OrdinalIgnoreCase"/>).
    /// </summary>
    public static class ParseHelper
    {
        /// <summary>
        /// Resolves a string against the active users (<see cref="IGame.GetActiveUsers"/>).
        /// Matches a numeric input by <see cref="IUser.GameSlotIndex"/>, otherwise tries
        /// <see cref="IUser.AccountName"/> then <see cref="IUser.Name"/>. The literal
        /// <c>"me"</c> resolves to <paramref name="self"/> (when non-null), and <c>"*"</c>
        /// resolves to all active users.
        /// </summary>
        /// <param name="input">The user identifier to resolve.</param>
        /// <param name="self">The invoking user, used to resolve <c>"me"</c>. Null skips that check.</param>
        /// <param name="comparison">The string comparison to use for name matching.</param>
        public static IEnumerable<IUser> ParseUsers(string input, IUser self = null,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(input))
                return [];

            IUser[] users = Game.GetActiveUsers();

            if (int.TryParse(input, out int slotIndex))
                return users.Where(u => u.GameSlotIndex == slotIndex);

            IEnumerable<IUser> byAccount = users.Where(u => string.Equals(u.AccountName, input, comparison));

            if (byAccount.Any())
                return byAccount;

            IEnumerable<IUser> byName = users.Where(u => string.Equals(u.Name, input, comparison));

            if (byName.Any())
                return byName;

            if (self != null && string.Equals(input, "me", comparison))
                return [self];

            if (string.Equals(input, "*", comparison))
                return users;

            return [];
        }

        /// <summary>
        /// Resolves a string against the players in the game. A numeric input indexes the
        /// array returned by <see cref="IGame.GetPlayers()"/> (which mostly matches game
        /// slots). Otherwise it delegates to <see cref="ParseUsers"/> (matching real players
        /// by account/name first, including the <c>"me"</c> shortcut), then falls back to
        /// matching <see cref="IObject.Name"/> across <see cref="IGame.GetPlayers()"/> to
        /// also catch externally spawned bots. The literal <c>"*"</c> resolves to all players.
        /// </summary>
        /// <param name="input">The player identifier to resolve.</param>
        /// <param name="self">The invoking user, forwarded to <see cref="ParseUsers"/> for the <c>"me"</c> shortcut.</param>
        /// <param name="comparison">The string comparison to use for name matching.</param>
        public static IEnumerable<IPlayer> ParsePlayers(string input, IUser self = null,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(input))
                return [];

            IPlayer[] players = Game.GetPlayers();

            if (int.TryParse(input, out int index))
                return index >= 0 && index < players.Length ? [players[index]] : [];

            if (!string.Equals(input, "*", comparison))
            {
                IEnumerable<IUser> users = ParseUsers(input, self, comparison);

                if (users.Any())
                    return users.Select(u => u.GetPlayer()).Where(p => p != null);
            }

            IEnumerable<IPlayer> byName = players
                .Where(p => string.Equals(p.Name, input, comparison));

            if (byName.Any())
                return byName;

            if (string.Equals(input, "*", comparison))
                return players;

            return [];
        }
    }
}
