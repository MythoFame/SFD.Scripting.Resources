using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SFD.Scripting.Resources.Generator;

internal static class Program
{
    internal static readonly string[] GameDirectories =
    [
        $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.steam/steam/steamapps/common/Superfighters Deluxe", // Linux
        $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.var/app/com.valvesoftware.Steam/.local/share/Steam/steamapps/common/Superfighters Deluxe", // Linux (flatpak)
        "C:/Program Files (x86)/Steam/steamapps/common/Superfighters Deluxe", // Windows
    ];

    internal static readonly IGenerator[] Generators =
    [
        new Tiles(),
        new Sounds()
    ];

    extension(Directory)
    {
        public static IEnumerable<string> EnumerateFiles(string[] directories, string searchPattern, SearchOption searchOption)
        {
            List<string> files = [];
            foreach (string directory in directories) files.AddRange(Directory.EnumerateFiles(directory, searchPattern, searchOption));

            return files;
        }

        public static string[] GetFiles(string[] directories, string searchPattern, SearchOption searchOption)
        {
            return [.. EnumerateFiles(directories, searchPattern, searchOption)];
        }
    }

    private static int Main(string[] args)
    {
        string? gameDir = null;
        string assetsSubDir = "Content/Data";
        string? repoDir = null;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--sfd-game-dir" when i + 1 < args.Length: gameDir = args[++i]; break;
                case "--assets-subdir" when i + 1 < args.Length: assetsSubDir = args[++i]; break;
                case "--repo-dir" when i + 1 < args.Length: repoDir = args[++i]; break;
                default:
                    Console.Error.WriteLine($"Unexpected argument '{args[i]}'. Expected --sfd-game-dir, --assets-subdir, or --repo-dir.");
                    return 1;
            }
        }

        if (string.IsNullOrWhiteSpace(repoDir))
        {
            Console.Error.WriteLine("No SfdRepoDir provided. Run the generator through MSBuild (Directory.Build.props sets SfdRepoDir), or pass --repo-dir <path>.");
            return 1;
        }

        string? dataRoot = ResolveDataDirectory(gameDir, assetsSubDir);
        if (dataRoot == null)
            return 1;

        string outputDir = Path.Combine(repoDir, "Utils");
        foreach (IGenerator generator in Generators) generator.Generate(dataRoot, outputDir);

        return 0;
    }

    private static string? ResolveDataDirectory(string? gameDir, string assetsSubDir)
    {
        if (!string.IsNullOrWhiteSpace(gameDir))
        {
            string dataRoot = Path.Combine(gameDir, assetsSubDir);
            if (!Directory.Exists(dataRoot))
            {
                Console.Error.WriteLine($"SFD data directory '{dataRoot}' not found. Check SfdGameDir (Directory.Build.props / --sfd-game-dir).");
                return null;
            }

            return dataRoot;
        }

        string[] candidates = [.. GameDirectories
            .Select(gameDir => Path.Combine(gameDir, assetsSubDir))
            .Where(Directory.Exists)];

        if (candidates.Length == 0)
        {
            Console.Error.WriteLine($"Couldn't find an SFD installation. Searched {GameDirectories.Length} locations for an assets sub-directory '{assetsSubDir}'.");
            Console.Error.WriteLine("Set SfdGameDir in Directory.Build.props (uncomment <SfdGameDir>), or pass --sfd-game-dir <path>.");
            return null;
        }

        if (candidates.Length > 1)
        {
            Console.Error.WriteLine("Multiple SFD installations found:");
            foreach (string candidate in candidates) Console.Error.WriteLine($"  - {candidate}");
            Console.Error.WriteLine("Set SfdGameDir in Directory.Build.props (uncomment <SfdGameDir>), or pass --sfd-game-dir <path> to disambiguate.");
            return null;
        }

        return candidates[0];
    }
}
