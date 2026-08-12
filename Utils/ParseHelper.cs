using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Reusable string parsers for common SFD types, intended for chat commands.
    /// All methods return an <see cref="IEnumerable{T}"/> and yield an empty
    /// collection when nothing matches. Which parsing operations to attempt is
    /// controlled by a <see cref="ParseFlags"/> bitmask, and name matching uses
    /// the supplied <see cref="StringComparison"/> (default <see cref="StringComparison.OrdinalIgnoreCase"/>).
    /// </summary>
    public static class ParseHelper
    {
        /// <summary>
        /// Bitmask of the parsing operations a parser should attempt. Flags are
        /// evaluated in a fixed, secure order regardless of their bit values — index,
        /// then account name, then name, and finally special tokens — so an identifier
        /// that collides with a literal token (e.g. a user named <c>"me"</c>) is still
        /// matched by its user data first.
        /// </summary>
        [Flags]
        public enum ParseFlags
        {
            /// <summary>No parsing operations are attempted.</summary>
            None = 0,

            /// <summary>
            /// Match a numeric input. For users this matches <see cref="IUser.GameSlotIndex"/>;
            /// for players it indexes the array returned by <see cref="IGame.GetPlayers()"/>,
            /// which tracks the game slots and continues into the remaining players.
            /// </summary>
            ByIndex = 1 << 0,

            /// <summary>Match a literal string against <see cref="IUser.AccountName"/>.</summary>
            ByAccountName = 1 << 1,

            /// <summary>Match a literal string against the player's <see cref="IObject.Name"/>.</summary>
            ByName = 1 << 2,

            /// <summary>
            /// Match reserved literal tokens: <c>"me"</c> (the invoking user, when supplied)
            /// and <c>"*"</c> (every user or player).
            /// </summary>
            SpecialTokens = 1 << 3,

            /// <summary>Every parsing operation, i.e. the historical "parse by everything" behavior.</summary>
            Everything = ByIndex | ByAccountName | ByName | SpecialTokens
        }

        /// <summary>
        /// Resolves a string against the active users (<see cref="IGame.GetActiveUsers"/>)
        /// using the enabled <paramref name="options"/>. In order: a numeric input matches
        /// <see cref="IUser.GameSlotIndex"/>, then <see cref="IUser.AccountName"/>, then
        /// <see cref="IUser.Name"/>, and finally the literal <c>"me"</c> (resolving to
        /// <paramref name="self"/> when non-null) and <c>"*"</c> (all active users).
        /// </summary>
        /// <param name="input">The user identifier to resolve.</param>
        /// <param name="self">The invoking user, used to resolve <c>"me"</c>. Null skips that check.</param>
        /// <param name="options">The parsing operations to attempt.</param>
        /// <param name="comparison">The string comparison to use for name matching.</param>
        public static IEnumerable<IUser> ParseUsers(string input, IUser self = null,
            ParseFlags options = ParseFlags.Everything,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(input))
                return [];

            IUser[] users = Game.GetActiveUsers();

            if (options.HasFlag(ParseFlags.ByIndex) && int.TryParse(input, out int slotIndex))
                return users.Where(u => u.GameSlotIndex == slotIndex);

            if (options.HasFlag(ParseFlags.ByAccountName))
            {
                IEnumerable<IUser> byAccount = users.Where(u => string.Equals(u.AccountName, input, comparison));

                if (byAccount.Any())
                    return byAccount;
            }

            if (options.HasFlag(ParseFlags.ByName))
            {
                IEnumerable<IUser> byName = users.Where(u => string.Equals(u.Name, input, comparison));

                if (byName.Any())
                    return byName;
            }

            if (options.HasFlag(ParseFlags.SpecialTokens))
            {
                if (self != null && string.Equals(input, "me", comparison))
                    return [self];

                if (string.Equals(input, "*", comparison))
                    return users;
            }

            return [];
        }

        /// <summary>
        /// Resolves a string against the players in the game using the enabled
        /// <paramref name="options"/>. In order: a numeric input indexes the array
        /// returned by <see cref="IGame.GetPlayers()"/>; the string is then delegated to
        /// <see cref="ParseUsers"/> (matching real players by account/name first), falling
        /// back to matching <see cref="IObject.Name"/> across the players to also catch
        /// externally spawned bots; finally the literal <c>"*"</c> resolves to all players.
        /// </summary>
        /// <param name="input">The player identifier to resolve.</param>
        /// <param name="self">The invoking user, forwarded to <see cref="ParseUsers"/> for the <c>"me"</c> shortcut.</param>
        /// <param name="options">The parsing operations to attempt.</param>
        /// <param name="comparison">The string comparison to use for name matching.</param>
        public static IEnumerable<IPlayer> ParsePlayers(string input, IUser self = null,
            ParseFlags options = ParseFlags.Everything,
            StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrEmpty(input))
                return [];

            IPlayer[] players = Game.GetPlayers();

            if (options.HasFlag(ParseFlags.ByIndex) && int.TryParse(input, out int index))
                return index >= 0 && index < players.Length ? [players[index]] : [];

            bool userParsingEnabled =
                options.HasFlag(ParseFlags.ByAccountName)
                || options.HasFlag(ParseFlags.ByName)
                || options.HasFlag(ParseFlags.SpecialTokens);

            if (userParsingEnabled && !string.Equals(input, "*", comparison))
            {
                IEnumerable<IUser> users = ParseUsers(input, self, options, comparison);

                if (users.Any())
                    return users.Select(u => u.GetPlayer()).Where(p => p != null);
            }

            if (options.HasFlag(ParseFlags.ByName))
            {
                IEnumerable<IPlayer> byName = players
                    .Where(p => string.Equals(p.Name, input, comparison));

                if (byName.Any())
                    return byName;
            }

            if (options.HasFlag(ParseFlags.SpecialTokens) && string.Equals(input, "*", comparison))
                return players;

            return [];
        }
    }
}
