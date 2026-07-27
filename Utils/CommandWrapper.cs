using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Wraps common <see cref="IGame.RunCommand"/> calls behind strongly-typed methods.
    /// Most commands only work when the script is loaded as a script extension —
    /// they are silently ignored when run as a map script.
    /// </summary>
    public static class CommandWrapper
    {
        /// <summary>
        /// Sets the slow-motion modifier. Equivalent to the <c>settime</c> chat command.
        /// </summary>
        /// <param name="time">The slow-motion time scale.</param>
        public static void SetSlowMotionModifier(float time) => Game.RunCommand($"settime {time}");

        /// <summary>
        /// Enables or disables slow-motion. Equivalent to the <c>slomo</c> chat command.
        /// </summary>
        /// <param name="slomo"><c>true</c> to enable slow-motion, <c>false</c> to disable.</param>
        public static void SetSlowMotion(bool slomo) => Game.RunCommand($"slomo {(slomo ? 1 : 0)}");

        /// <summary>
        /// Enables or disables infinite ammo. Equivalent to the <c>infinite_ammo</c> chat command.
        /// </summary>
        /// <param name="infiniteAmmo"><c>true</c> to enable infinite ammo, <c>false</c> to disable.</param>
        public static void SetInfiniteAmmo(bool infiniteAmmo) => Game.RunCommand($"infinite_ammo {(infiniteAmmo ? 1 : 0)}");

        /// <summary>
        /// Enables or disables infinite life. Equivalent to the <c>infinite_life</c> chat command.
        /// </summary>
        /// <param name="infiniteLife"><c>true</c> to enable infinite life, <c>false</c> to disable.</param>
        public static void SetInfiniteLife(bool infiniteLife) => Game.RunCommand($"infinite_life {(infiniteLife ? 1 : 0)}");

        /// <summary>
        /// Enables or disables infinite energy. Equivalent to the <c>infinite_energy</c> chat command.
        /// </summary>
        /// <param name="infiniteEnergy"><c>true</c> to enable infinite energy, <c>false</c> to disable.</param>
        public static void SetInfiniteEnergy(bool infiniteEnergy) => Game.RunCommand($"infinite_energy {(infiniteEnergy ? 1 : 0)}");

        /// <summary>
        /// Sets the starting life for players. Equivalent to the <c>startlife</c> chat command.
        /// </summary>
        /// <param name="life">The starting life value (1–100).</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="life"/> is outside the 1–100 range.</exception>
        public static void SetStartLife(int life)
        {
            if (life < 1 || life > 100) throw new ArgumentOutOfRangeException(nameof(life), "Must be between 1 and 100.");

            Game.RunCommand($"startlife {life}");
        }

        /// <summary>
        /// Sets the starting weapon items for players. Equivalent to the <c>setstartitems</c> chat command.
        /// </summary>
        /// <param name="weaponItems">The weapon items to grant on spawn.</param>
        public static void SetStartItems(IEnumerable<WeaponItem> weaponItems)
        {
            string items = string.Join(" ", weaponItems.Select(item => (int)item));

            Game.RunCommand($"setstartitems {items}");
        }

        /// <summary>
        /// Resets all active cheats. Equivalent to the <c>clear</c> chat command.
        /// </summary>
        public static void Clear() => Game.RunCommand("clear");

        /// <summary>
        /// Restarts the current fight immediately. Equivalent to the <c>rs</c> (restart) chat command.
        /// </summary>
        public static void Restart() => Game.RunCommand("rs");

        /// <summary>
        /// Advances to the next map in the rotation. Equivalent to the <c>nextmap</c> chat command.
        /// </summary>
        public static void NextMap() => Game.RunCommand("nextmap");

        /// <summary>
        /// Changes the map category. Equivalent to the <c>changemapcategory</c> chat command.
        /// </summary>
        /// <param name="category">The map category to switch to.</param>
        public static void ChangeMapCategory(MapType category) => Game.RunCommand($"changemapcategory {category}");

        /// <summary>
        /// Changes the map for the next fight. Equivalent to the <c>changemap</c> chat command.
        /// </summary>
        /// <param name="map">The name of the map to load.</param>
        public static void ChangeMap(string map) => Game.RunCommand($"changemap {map}");

        /// <summary>
        /// Starts a script by name. Equivalent to the <c>startscript</c> chat command.
        /// </summary>
        /// <param name="script">The name of the script to start.</param>
        public static void StartScript(string script) => Game.RunCommand($"startscript {script}");

        /// <summary>
        /// Stops a script by name. Equivalent to the <c>stopscript</c> chat command.
        /// </summary>
        /// <param name="script">The name of the script to stop.</param>
        public static void StopScript(string script) => Game.RunCommand($"stopscript {script}");

        /// <summary>
        /// Sets the current campaign chapter. Equivalent to the <c>setchapter</c> chat command.
        /// </summary>
        /// <param name="chapter">The chapter number.</param>
        /// <exception cref="InvalidOperationException">Thrown when the current map type is not <see cref="MapType.Campaign"/>.</exception>
        public static void SetChapter(ushort chapter)
        {
            if (Game.GetMapType() != MapType.Campaign)
                throw new InvalidOperationException(
                    "The chapter can only be set while playing a campaign.");

            Game.RunCommand($"setchapter {chapter}");
        }

        /// <summary>
        /// Advances to the next campaign chapter. Equivalent to the <c>nextchapter</c> chat command.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the current map type is not <see cref="MapType.Campaign"/>.</exception>
        public static void NextChapter()
        {
            if (Game.GetMapType() != MapType.Campaign)
                throw new InvalidOperationException(
                    "The chapter can only be set while playing a campaign.");

            Game.RunCommand($"nextchapter");
        }

        /// <summary>
        /// Sets the map rotation interval. Equivalent to the <c>maprotation</c> chat command.
        /// </summary>
        /// <param name="amount">The number of fights before rotation.</param>
        public static void SetMapRotation(ushort amount) => Game.RunCommand($"maprotation {amount}");

        /// <summary>
        /// Sets the map rotation mode. Equivalent to the <c>maprotation</c> chat command.
        /// </summary>
        /// <param name="mode">The rotation mode (<see cref="MapRotationMode"/>).</param>
        public static void SetMapRotation(MapRotationMode mode) => Game.RunCommand($"maprotation {GetMapRotationModeValue(mode)}");

        /// <summary>
        /// Sets the map rotation mode and interval. Equivalent to the <c>maprotation</c> chat command.
        /// </summary>
        /// <param name="amount">The number of fights before rotation.</param>
        /// <param name="mode">The rotation mode (<see cref="MapRotationMode"/>).</param>
        public static void SetMapRotation(ushort amount, MapRotationMode mode) => Game.RunCommand($"maprotation {GetMapRotationModeValue(mode)} {amount}");

        /// <summary>
        /// Sets the difficulty for campaign maps. Equivalent to the <c>setdifficulty</c> chat command.
        /// </summary>
        /// <param name="difficulty">The difficulty level.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="difficulty"/> is <see cref="DifficultyLevel.None"/>.</exception>
        public static void SetDifficulty(DifficultyLevel difficulty)
        {
            if (difficulty == DifficultyLevel.None) throw new ArgumentOutOfRangeException(nameof(difficulty));

            Game.RunCommand($"setdifficulty {(int)difficulty}");
        }

        /// <summary>
        /// Sets the round time limit. Equivalent to the <c>timelimit</c> chat command.
        /// </summary>
        /// <param name="time">The time limit in seconds. Pass 0 to disable.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="time"/> is outside the 30–600 range and not 0.</exception>
        public static void SetTimeLimit(int time)
        {
            if ((time < 30 || time > 600) && time != 0) throw new ArgumentOutOfRangeException(nameof(time), "Must be between 30 and 600 or 0.");

            Game.RunCommand($"timelimit {time}");
        }

        /// <summary>
        /// Shuffles the teams for the next fight. Equivalent to the <c>shuffleteams</c> chat command.
        /// </summary>
        public static void ShuffleTeams() => Game.RunCommand("shuffleteams");

        /// <summary>
        /// Shuffles the teams every specified number of fights. Equivalent to the <c>shuffleteams</c> chat command.
        /// </summary>
        /// <param name="amount">The number of fights between each shuffle.</param>
        public static void ShuffleTeams(ushort amount) => Game.RunCommand($"shuffleteams {amount}");

        /// <summary>
        /// Enables or disables global chat. Equivalent to the <c>chat</c> chat command.
        /// </summary>
        /// <param name="enabled"><c>true</c> to enable global chat, <c>false</c> to disable.</param>
        /// <exception cref="InvalidOperationException">Thrown when the game type is <see cref="GameType.Offline"/>.</exception>
        public static void SetGlobalChat(bool enabled)
        {
            if (Game.GetGameType() == GameType.Offline) throw new InvalidOperationException("This operation requires an online game.");

            Game.RunCommand($"chat {(enabled ? 1 : 0)}");
        }

        /// <summary>
        /// Bans a player. Equivalent to the <c>ban</c> chat command.
        /// </summary>
        /// <param name="player">The player to ban.</param>
        /// <exception cref="InvalidOperationException">Thrown when the game type is <see cref="GameType.Offline"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the player is a host, moderator, or bot.</exception>
        public static void Ban(IPlayer player)
        {
            if (Game.GetGameType() == GameType.Offline) throw new InvalidOperationException("This operation requires an online game.");

            IUser user = player.GetUser();

            if (user.IsHost || user.IsModerator || user.IsBot) throw new ArgumentException("The player must not be privileged or a bot.");

            Game.RunCommand($"ban {user.GameSlotIndex}");
        }

        /// <summary>
        /// Kicks a player. Equivalent to the <c>kick</c> chat command.
        /// </summary>
        /// <param name="player">The player to kick.</param>
        /// <exception cref="InvalidOperationException">Thrown when the game type is <see cref="GameType.Offline"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the player is a host, moderator, or bot.</exception>
        public static void Kick(IPlayer player)
        {
            if (Game.GetGameType() == GameType.Offline) throw new InvalidOperationException("This operation requires an online game.");

            IUser user = player.GetUser();

            if (user.IsHost || user.IsModerator || user.IsBot) throw new ArgumentException("The player must not be privileged or a bot.");

            Game.RunCommand($"kick {user.GameSlotIndex}");
        }

        /// <summary>
        /// Kicks a player for a specified duration. Equivalent to the <c>kick</c> chat command with a time parameter.
        /// </summary>
        /// <param name="player">The player to kick.</param>
        /// <param name="minutes">The kick duration in minutes. Must be 60 or less.</param>
        /// <exception cref="InvalidOperationException">Thrown when the game type is <see cref="GameType.Offline"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the player is a host, moderator, or bot.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minutes"/> exceeds 60.</exception>
        public static void Kick(IPlayer player, int minutes)
        {
            if (Game.GetGameType() == GameType.Offline) throw new InvalidOperationException("This operation requires an online game.");

            if (minutes > 60) throw new ArgumentOutOfRangeException(nameof(minutes), "Must be below 60.");

            IUser user = player.GetUser();

            if (user.IsHost || user.IsModerator || user.IsBot) throw new ArgumentException("The player must not be privileged or a bot.");

            Game.RunCommand($"kick {minutes} {user.GameSlotIndex}");
        }

        /// <summary>
        /// Sets the maximum allowed ping. Equivalent to the <c>maxping</c> chat command.
        /// </summary>
        /// <param name="ping">The maximum ping threshold. Pass 0 to disable.</param>
        /// <exception cref="InvalidOperationException">Thrown when the game type is <see cref="GameType.Offline"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="ping"/> is outside the 50–500 range and not 0.</exception>
        public static void SetMaximumPing(int ping)
        {
            if (Game.GetGameType() == GameType.Offline) throw new InvalidOperationException("This operation requires an online game.");

            if ((ping < 50 || ping > 500) && ping != 0) throw new ArgumentOutOfRangeException(nameof(ping), "Must be between 50 and 500 or 0.");

            Game.RunCommand($"maxping {ping}");
        }

        /// <summary>
        /// Sets the maximum idle time before a player is kicked. Equivalent to the <c>kickidle</c> chat command.
        /// </summary>
        /// <param name="seconds">The idle timeout in seconds. Pass 0 to disable.</param>
        /// <exception cref="InvalidOperationException">Thrown when the game type is <see cref="GameType.Offline"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="seconds"/> is outside the 30–600 range and not 0.</exception>
        public static void SetMaximumIdleTime(int seconds)
        {
            if (Game.GetGameType() == GameType.Offline) throw new InvalidOperationException("This operation requires an online game.");

            if ((seconds < 30 || seconds > 600) && seconds != 0) throw new ArgumentOutOfRangeException(nameof(seconds), "Must be between 30 and 600 or 0.");

            Game.RunCommand($"kickidle {seconds}");
        }

        /// <summary>
        /// Determines how maps are chosen from the rotation.
        /// </summary>
        public enum MapRotationMode
        {
            Disabled,
            Vote,
            Sequential,
            Random
        }

        private static string GetMapRotationModeValue(MapRotationMode mode) =>
            mode switch
            {
                MapRotationMode.Disabled => "a",
                MapRotationMode.Vote => "b",
                MapRotationMode.Sequential => "c",
                MapRotationMode.Random => "d",
                _ => throw new ArgumentOutOfRangeException(nameof(mode))
            };
    }
}
