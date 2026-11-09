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
        /// Splits a raw command-argument string on whitespace, keeping
        /// double-quoted sections together as single tokens with the quotes
        /// stripped (e.g. <c>kill "my player" gib</c> yields
        /// <c>"my player"</c> and <c>"gib"</c>). Unmatched quotes run to the
        /// end of the string. Yields nothing for null or blank input.
        /// </summary>
        /// <param name="input">The raw argument string to tokenize.</param>
        public static IEnumerable<string> SplitArguments(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                yield break;

            System.Text.StringBuilder token = new();
            bool inQuotes = false;
            bool hasToken = false;

            foreach (char c in input)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    hasToken = true;
                }
                else if (!inQuotes && char.IsWhiteSpace(c))
                {
                    if (hasToken)
                    {
                        yield return token.ToString();
                        token.Clear();
                        hasToken = false;
                    }
                }
                else
                {
                    token.Append(c);
                    hasToken = true;
                }
            }

            if (hasToken)
                yield return token.ToString();
        }

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
            /// Match a numeric input or inclusive <c>start..end</c> range. Range endpoints
            /// must be non-negative integers with <c>start</c> less than or equal to
            /// <c>end</c>. For users this matches <see cref="IUser.GameSlotIndex"/>;
            /// for players it matches the associated user's <see cref="IUser.GameSlotIndex"/>
            /// when available, falling back to the position in the array returned by
            /// <see cref="IGame.GetPlayers()"/> for players without an associated user.
            /// </summary>
            ByIndex = 1 << 0,

            /// <summary>Match a literal string against <see cref="IUser.AccountName"/>.</summary>
            ByAccountName = 1 << 1,

            /// <summary>Match a literal string against the player's <see cref="IObject.Name"/>.</summary>
            ByName = 1 << 2,

            /// <summary>
            /// Match reserved literal tokens: <c>"me"</c> (the invoking user, when supplied),
            /// <c>"*"</c> (every user or player) and <c>"?"</c> (one random user or player).
            /// </summary>
            SpecialTokens = 1 << 3,

            /// <summary>Every parsing operation, i.e. the historical "parse by everything" behavior.</summary>
            Everything = ByIndex | ByAccountName | ByName | SpecialTokens
        }

        private static bool TryParseIndexRange(string input, out int start, out int end)
        {
            start = 0;
            end = -1;

            string[] parts = input.Split("..");

            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out start) || !int.TryParse(parts[1], out end))
                return false;

            return start >= 0 && start <= end;
        }

        private static IEnumerable<IPlayer> ParsePlayersBySlot(IPlayer[] players, int start, int end)
        {
            return players.Where((player, playerIndex) =>
            {
                IUser user = player.GetUser();

                if (user is not null)
                    return user.GameSlotIndex >= start && user.GameSlotIndex <= end;

                return playerIndex >= start && playerIndex <= end;
            });
        }

        /// <summary>
        /// Resolves a string against the active users (<see cref="IGame.GetActiveUsers"/>)
        /// using the enabled <paramref name="options"/>. In order: a numeric input matches
        /// <see cref="IUser.GameSlotIndex"/>, an inclusive range selects users whose slot
        /// falls in that interval, then <see cref="IUser.AccountName"/>, then
        /// <see cref="IUser.Name"/>, and finally the literal <c>"me"</c> (resolving to
        /// <paramref name="self"/> when non-null), <c>"*"</c> (all active users) and
        /// <c>"?"</c> (one random active user).
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

            if (options.HasFlag(ParseFlags.ByIndex) && TryParseIndexRange(input, out int startSlot, out int endSlot))
                return users.Where(u => u.GameSlotIndex >= startSlot && u.GameSlotIndex <= endSlot);

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

                if (string.Equals(input, "?", comparison))
                    return users.Length > 0 ? [users[Random.Shared.Next(users.Length)]] : [];
            }

            return [];
        }

        /// <summary>
        /// Resolves a string against the players in the game using the enabled
        /// <paramref name="options"/>. In order: a numeric input or inclusive range
        /// selects players by associated-user slot when available, falling back to array
        /// positions for players without an associated user; the string is then delegated to
        /// <see cref="ParseUsers"/> (matching real players by account/name first), falling
        /// back to matching <see cref="IObject.Name"/> across the players to also catch
        /// externally spawned bots; finally the literal <c>"*"</c> resolves to all players
        /// and <c>"?"</c> resolves to one random player.
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
                return ParsePlayersBySlot(players, index, index);

            if (options.HasFlag(ParseFlags.ByIndex) && TryParseIndexRange(input, out int startIndex, out int endIndex))
                return ParsePlayersBySlot(players, startIndex, endIndex);

            bool userParsingEnabled =
                options.HasFlag(ParseFlags.ByAccountName)
                || options.HasFlag(ParseFlags.ByName)
                || options.HasFlag(ParseFlags.SpecialTokens);

            if (userParsingEnabled && !string.Equals(input, "*", comparison) && !string.Equals(input, "?", comparison))
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

            if (options.HasFlag(ParseFlags.SpecialTokens) && string.Equals(input, "?", comparison))
                return players.Length > 0 ? [players[Random.Shared.Next(players.Length)]] : [];

            if (options.HasFlag(ParseFlags.SpecialTokens) && string.Equals(input, "*", comparison))
                return players;

            return [];
        }
    }
}
