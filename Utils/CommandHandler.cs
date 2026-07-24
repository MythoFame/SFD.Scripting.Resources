using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Tracks registered chat commands and dispatches incoming user messages to
    /// their associated callbacks. Call <see cref="Initialize"/> once to subscribe
    /// to user message events; <see cref="Command"/> instances can be added to
    /// <see cref="ActiveCommands"/> before or after initialization.
    /// </summary>
    public static class CommandHandler
    {
        private static bool _initialized = false;

        /// <summary>
        /// All commands currently registered with the handler. Add a <see cref="Command"/>
        /// to this list to make it eligible for activation by users in chat.
        /// </summary>
        public static readonly List<Command> ActiveCommands = [];

        /// <summary>
        /// Subscribes the handler to user message events. Calling this more than once is a no-op
        /// and logs a warning to the console.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
            {
                Game.WriteToConsoleF("CommandHandler is already initialized.");

                return;
            }

            Game.Events.StartUserMessageCallback(OnUserMessage);

            _initialized = true;

            Game.WriteToConsoleF("CommandHandler initialized.");
        }

        /// <summary>
        /// Create a command instance using this function as a parameter for an automatic help command.
        /// </summary>
        public static void DisplayHelp(UserMessageCallbackArgs args)
        {
            IUser user = args.User;

            Game.ShowChatMessage("Available commands:", Color.Green, user.UserIdentifier);

            IOrderedEnumerable<Command> commands = ActiveCommands
            .OrderBy(cmd => cmd.ModeratorOnly)
            .ThenBy(cmd => cmd.Name);

            foreach (Command command in commands)
            {
                if (command.ModeratorOnly && !user.IsModerator) continue;

                string displayTxt = $"/{command.Name} ";

                if (command.Description != null)
                    displayTxt += command.Description;

                Game.ShowChatMessage(displayTxt, command.ModeratorOnly ? Color.Yellow : Color.Green, args.User.UserIdentifier);
            }
        }

        /// <summary>
        /// Invoked for every user message. When the message is a command, locates the
        /// matching <see cref="Command"/> in <see cref="ActiveCommands"/>, enforces its
        /// <see cref="Command.ModeratorOnly"/> permission, and fires its callback.
        /// </summary>
        private static void OnUserMessage(UserMessageCallbackArgs args)
        {
            if (!args.IsCommand) return;

            Command commandActivated = ActiveCommands
              .FirstOrDefault(c => c.Name == args.Command);

            if (commandActivated == null) return;

            IUser user = args.User;

            if (!user.IsModerator && commandActivated.ModeratorOnly)
            {
                Game.ShowChatMessage("You don't have permission to use this command.",
                Color.Red, user.UserIdentifier);

                return;
            }

            commandActivated.OnCommand.Invoke(args);
        }

        /// <summary>
        /// Represents a single chat command that the <see cref="CommandHandler"/> can dispatch.
        /// </summary>
        public class Command
        {
            private string _name = string.Empty;

            /// <summary>
            /// The case-insensitive name of the command. Stored and compared in upper-case.
            /// </summary>
            public string Name
            {
                get => _name; set => _name = value.ToUpper();
            }

            /// <summary>
            /// Whether the command requires a moderator to execute it. By default false.
            /// </summary>
            public bool ModeratorOnly = false;

            /// <summary>
            /// The human-readable description to show when the user requests command help.
            /// </summary>
            public string Description = null;

            /// <summary>
            /// The action executed when a user issues this command in chat. Receives the
            /// original <see cref="UserMessageCallbackArgs"/> containing the sender and arguments.
            /// </summary>
            public Action<UserMessageCallbackArgs> OnCommand = null;

            /// <summary>
            /// Creates a new <see cref="Command"/> with the given name and callback.
            /// </summary>
            /// <param name="name">The name of the command, as typed in chat (without leading slash).</param>
            /// <param name="onCommand">The callback to invoke when the command is used.</param>
            public Command(string name, Action<UserMessageCallbackArgs> onCommand)
            {
                Name = name;
                OnCommand = onCommand;
            }
        }
    }
}
