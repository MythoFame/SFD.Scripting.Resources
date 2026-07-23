namespace SFD.Scripting.Resources;

public partial class GameScript : GameScriptInterfaceExtended
{
    /// <summary>
    /// Creates an instance of the specified type by invoking its public constructor that
    /// matches the given argument list, returning the result cast to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The interface or base type to cast the created instance to.</typeparam>
    /// <param name="type">The concrete <see cref="Type"/> to instantiate.</param>
    /// <param name="args">Arguments to pass to the matching constructor.</param>
    /// <returns>A new instance of <paramref name="type"/> cast to <typeparamref name="T"/>,
    /// or <c>null</c> if <paramref name="type"/> is <c>null</c>.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="type"/> has no public constructor matching the supplied arguments.
    /// </exception>
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
