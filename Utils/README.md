<div align="center">

[![Superfighters Deluxe Logo](../.github/assets/SFD_titleLoop.gif)](https://store.steampowered.com/app/855860)

# Superfighters Deluxe Scripting Resources

Resources, utilities, helper libraries, and code snippets for developing scripts for Superfighters Deluxe.

[![License](https://img.shields.io/github/license/dsafxP/SFD.ScriptTools)](../LICENSE.txt)

</div>

## [CommandHandler](CommandHandler.cs)

A registry and dispatcher for chat commands. Register `Command` instances in `ActiveCommands`, call `Initialize` once, and the handler routes incoming user messages to the matching command's callback. Commands are matched case-insensitively, and `ModeratorOnly` commands are gated behind a permission check. Provides a `DisplayHelp` helper that lists all commands a user is allowed to run.

Example usage:

```cs
public void OnStartup()
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

    // Automatic help command
    CommandHandler.ActiveCommands.Add(new("T_HELP", CommandHandler.DisplayHelp) {
        Description = "- Displays command help."
    });
}

private void Test(UserMessageCallbackArgs args) => Game.WriteToConsoleF("Hello World!");
```

## [CreateInstance](CreateInstance.cs)

A generic factory helper that instantiates a `Type` by reflecting over its public constructor and casting the result to `T`. Throws `InvalidOperationException` when no matching constructor exists.

## [CustomProjectile](CustomProjectile.cs)

A fully customizable projectile that travels in a straight line, performs its own ray-cast collision each update, and fires `OnPlayerHit`/`OnObjectHit` callbacks on impact. Supports piercing (multiple hits before disabling), wallbanging through indestructible geometry, a maximum travel distance, a trailing effect, and a copy constructor for easy templating.

## [HomingProjectile](HomingProjectile.cs)

Extends `CustomProjectile` with self-steering behavior. Each update it rotates its direction towards a target position — by default the closest living enemy of `Shooter` — with `Homing` (0–1) controlling how aggressively it turns. Override `GetHomingTargetPosition` to implement custom targeting.

## [PlayerHelper](PlayerHelper.cs)

Generic utilities for `IPlayer`, such as unsticking players from geometry and querying firing state.

## [PointShape](PointShape.cs)

Static helpers that generate collections of `Vector2` points along common shapes — trails, circles, squares/polygons, swirls and waves — plus a random-in-area generator. Each method invokes a callback for every produced point, so they can be used to drive effects, spawns, or any point-wise operation. Includes a `DegreesToRadians` conversion helper.

<img alt="PointShape" src="../.github/assets/Shape.gif" />

## [Vector2Helper](Vector2Helper.cs)

A math utility class for `Vector2` offering operations not built into the SFD API: angles, dot/cross products, reflection and bouncing, projection, rotation, clamping, length limiting, move-toward, and more. Also exposes the `Up`/`Down`/`Left`/`Right` unit vectors.
