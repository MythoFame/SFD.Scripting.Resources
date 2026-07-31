using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Displays multiple chat messages in sequence. Each message in <paramref name="messages"/>
    /// is sent as its own line via <see cref="IGame.ShowChatMessage(string)"/>.
    /// </summary>
    /// <param name="messages">The messages to display, each representing a new line in the chat.</param>
    public static void ShowChatMessages(IEnumerable<string> messages)
    {
        foreach (string message in messages)
            Game.ShowChatMessage(message);
    }

    /// <summary>
    /// Displays multiple chat messages in sequence with the given color. Each message in
    /// <paramref name="messages"/> is sent as its own line via
    /// <see cref="IGame.ShowChatMessage(string, Color)"/>.
    /// </summary>
    /// <param name="messages">The messages to display, each representing a new line in the chat.</param>
    /// <param name="color">The color applied to every message.</param>
    public static void ShowChatMessages(IEnumerable<string> messages, Color color)
    {
        foreach (string message in messages)
            Game.ShowChatMessage(message, color);
    }

    /// <summary>
    /// Displays multiple chat messages in sequence to a specific user. Each message in
    /// <paramref name="messages"/> is sent as its own line via
    /// <see cref="IGame.ShowChatMessage(string, int)"/>.
    /// </summary>
    /// <param name="messages">The messages to display, each representing a new line in the chat.</param>
    /// <param name="userIdentifier">The identifier of the user to receive the messages.</param>
    public static void ShowChatMessages(IEnumerable<string> messages, int userIdentifier)
    {
        foreach (string message in messages)
            Game.ShowChatMessage(message, userIdentifier);
    }

    /// <summary>
    /// Displays multiple chat messages in sequence to a specific user with the given color.
    /// Each message in <paramref name="messages"/> is sent as its own line via
    /// <see cref="IGame.ShowChatMessage(string, Color, int)"/>.
    /// </summary>
    /// <param name="messages">The messages to display, each representing a new line in the chat.</param>
    /// <param name="color">The color applied to every message.</param>
    /// <param name="userIdentifier">The identifier of the user to receive the messages.</param>
    public static void ShowChatMessages(IEnumerable<string> messages, Color color, int userIdentifier)
    {
        foreach (string message in messages)
            Game.ShowChatMessage(message, color, userIdentifier);
    }
}
