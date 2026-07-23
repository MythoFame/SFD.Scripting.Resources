namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    public static T CreateInstance<T>(Type type, params object[] args) where T : class
    {
        if (type == null)
            return null;

        Type[] argTypes = [.. args.Select(a => a.GetType())];

        var ctor = type.GetConstructor(argTypes);

        return ctor == null
            ? throw new InvalidOperationException(
              $"Type '{type.Name}' is missing a matching public constructor."
            )
            : ctor.Invoke(args) as T;
    }
}
