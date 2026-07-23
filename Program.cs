using SFDGameScriptInterface;

namespace SFD.Scripting.Resources;

public abstract class GameScriptInterfaceExtended : GameScriptInterface
{
    protected static readonly IGame Game;
}

public partial class GameScript : GameScriptInterfaceExtended
{
    // ...
}
