# [CommandHandler](CommandHandler.cs)

A registry and dispatcher for chat commands. Register `Command` instances in `ActiveCommands`, call `Initialize` once, and the handler routes incoming user messages to the matching command's callback. Commands are matched case-insensitively, and `ModeratorOnly`/`HostOnly` commands are gated behind a permission check. Provides a `DisplayHelp` helper that lists all commands a user is allowed to run.

Example usage:

```cs
public static void OnStartup()
{
    // Initialize
    CommandHandler.Initialize();

    // Normal command
    CommandHandler.ActiveCommands.Add(new("TEST", Test) {
        Description = "- This is a test command!"
    });

    // Moderator command
    CommandHandler.ActiveCommands.Add(new("MOD", Test) {
        Description = "- This is a moderator only command!",
        ModeratorOnly = true
    });

    // Host command
    CommandHandler.ActiveCommands.Add(new("HOST", Test) {
        Description = "- This is a host only command!",
        HostOnly = true
    });

    // Automatic help command
    CommandHandler.ActiveCommands.Add(new("T_HELP", CommandHandler.DisplayHelp) {
        Description = "- Displays command help."
    });
}

private static void Test(UserMessageCallbackArgs args) => Game.WriteToConsoleF("Hello World!");
```

# [CommandWrapper](CommandWrapper.cs)

A strongly-typed wrapper around `Game.RunCommand` for common chat commands — cheats, map rotation, player management, and more. Most commands only work when the script is loaded as a script extension and are silently ignored when run as a map script.

# [CreateInstance](CreateInstance.cs)

A generic factory helper that instantiates a `Type` by reflecting over its public constructor and casting the result to `T`. Throws `InvalidOperationException` when no matching constructor exists.

# [CustomProjectile](CustomProjectile.cs)

A fully customizable projectile that travels in a straight line, performs its own ray-cast collision each update, and fires `OnPlayerHit`/`OnObjectHit` callbacks on impact. Supports piercing (multiple hits before disabling), wallbanging through indestructible geometry, a maximum travel distance, a trailing effect, and a copy constructor for easy templating.

# [GetRandomWeaponFromType](GetRandomWeaponFromType.cs)

Returns a random `WeaponItem` whose `WeaponItemType` matches the given category. Internally draws random weapons via `Game.GetRandomWeaponItem` and spawns them transiently to inspect their type, retrying until a match is found.

# [HomingProjectile](HomingProjectile.cs)

Extends `CustomProjectile` with self-steering behavior. Each update it rotates its direction towards a target position — by default the closest living enemy of `Shooter` — with `Homing` (0–1) controlling how aggressively it turns. Override `GetHomingTargetPosition` to implement custom targeting.

# [PlayerHelper](PlayerHelper.cs)

Generic utilities for `IPlayer`, such as unsticking players from geometry and querying firing state.

# [PointShape](PointShape.cs)

Static helpers that generate collections of `Vector2` points along common shapes — trails, circles, squares/polygons, swirls and waves — plus a random-in-area generator. Each method invokes a callback for every produced point, so they can be used to drive effects, spawns, or any point-wise operation. Includes a `DegreesToRadians` conversion helper.

<img alt="PointShape" src="../.github/assets/Shape.gif" />

# [ShowChatMessages](ShowChatMessages.cs)

Batch wrapper around `Game.ShowChatMessage` that accepts an `IEnumerable<string>` of messages and displays each one as its own line in the chat. Mirrors the four overloads of the underlying API: bare, colored, user-targeted, and colored-user-targeted.

# [Vector2Helper](Vector2Helper.cs)

A math utility class for `Vector2` offering operations not built into the SFD API: angles, dot/cross products, reflection and bouncing, projection, rotation, clamping, length limiting, move-toward, and more. Also exposes the `Up`/`Down`/`Left`/`Right` unit vectors.
