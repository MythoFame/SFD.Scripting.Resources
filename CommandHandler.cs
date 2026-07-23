using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    public static class CommandHandler
    {
        private static bool _initialized = false;

        public static readonly List<Command> ActiveCommands = [];

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

        private static void OnUserMessage(UserMessageCallbackArgs args)
        {
            if (!args.IsCommand)
                return;

            Command commandActivated = ActiveCommands
              .FirstOrDefault(c => c.Name == args.Command);

            commandActivated?.OnCommand.Invoke(args);
        }

        public class Command
        {
            private string _name = string.Empty;

            public string Name
            {
                get => _name; set => _name = value.ToUpper();
            }

            public Action<UserMessageCallbackArgs> OnCommand = null;

            public Command(string name, Action<UserMessageCallbackArgs> onCommand)
            {
                Name = name;
                OnCommand = onCommand;
            }
        }
    }
}
