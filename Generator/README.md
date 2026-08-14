# Generator

Regenerates the auto-generated `Utils/Tiles.cs` and `Utils/Sounds.cs` databases from your local SFD installation.

```sh
dotnet build -t:GenerateSourceDb
```

Run from the `Generator/` directory. Requires an SFD install (set `SfdGameDir` in `Directory.Build.props` if the default search fails).
