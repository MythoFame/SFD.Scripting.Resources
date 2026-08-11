<div align="center">

[![Superfighters Deluxe Logo](https://raw.githubusercontent.com/MythoFame/.github/refs/heads/master/assets/SFD_titleLoop.gif)](https://store.steampowered.com/app/855860)

# Superfighters Deluxe Scripting Resources

Resources, utilities, helper libraries, and code snippets for developing scripts for Superfighters Deluxe

[![GitHub License](https://img.shields.io/github/license/MythoFame/SFD.Scripting.Resources)](LICENSE)

</div>

## 🛠️ Utils

A collection of reusable helper classes for Superfighters Deluxe scripting — ranging from chat command dispatch and projectile behavior to Vector2 math and point-shape generators. Note that some utilities may depend on others within this collection.

**Available utilities:**

- `BouncingProjectile`
- `CommandHandler`
- `CommandWrapper`
- `Companions`
- `CreateInstance`
- `CustomProjectile`
- `EffectNamesExtra`
- `GetRandomWeaponFromType`
- `HomingProjectile`
- `NodeProjectile`
- `ParseHelper`
- `PlayerHelper`
- `ProjectileHelper`
- `PointShape`
- `ShowChatMessages`
- `Vector2Helper`

See [`Utils/README.md`](Utils/README.md) for detailed documentation on each utility.

## 📢 Events

Custom callbacks that wrap the existing SFD script API events, providing higher-level notifications (such as a player killing another, or the game ending) without reimplementing the boilerplate each time. They follow the legacy `Events.*Callback` lifecycle: call `Start` to register and `Stop` to unsubscribe.

**Available events:**

- `GameOverCallback`
- `PlayerKillCallback`

See [`Events/README.md`](Events/README.md) for detailed documentation on each event.
