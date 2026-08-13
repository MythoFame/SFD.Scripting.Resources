using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator;

internal static class Program
{
    internal const string AssetsDirectory = "Superfighters Deluxe/Content/Data";

    internal static readonly string[] GameDirectories =
    [
        $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}/.steam/steam/steamapps/common", // Linux
        "/opt/steam/steamapps/common", // Linux
        "C:/Program Files (x86)/Steam/steampapps/common", // Windows
        "C:/Steam/steampapps/common", // Windows
        "D:/Steam/steampapps/common" // Windows
    ];

    internal static readonly IGenerator[] Generators =
    [
        new Tiles(),
        new Sounds()
    ];

    extension(ICollection collection)
    {
        public bool IsLast(int index)
        {
            return index == collection.Count - 1;
        }
    }

    extension(Directory)
    {
        public static IEnumerable<string> EnumerateFiles(string[] directories, string searchPattern, SearchOption searchOption)
        {
            List<string> files = new List<string>();
            foreach (string directory in directories) files.AddRange(Directory.EnumerateFiles(directory, searchPattern, searchOption));

            return files;
        }

        public static string[] GetFiles(string[] directories, string searchPattern, SearchOption searchOption)
        {
            return EnumerateFiles(directories, searchPattern, searchOption).ToArray();
        }
    }

    private static int Main()
    {
        Environment.CurrentDirectory = "../../../../"; // exit from Generator/bin/(Debug|Release)/net10.0
        Console.Write(Environment.CurrentDirectory);

        string path = string.Empty;

        foreach ((int index, string directory) in GameDirectories.Index())
        {
            path = $"{directory}/{AssetsDirectory}";

            if (Directory.Exists(path))
                break;

            if (GameDirectories.IsLast(index))
            {
                Console.WriteLine("Couldn't find SFD directory.");
                return 1;
            }
        }

        foreach (IGenerator generator in Generators) generator.Generate(path);

        return 0;
    }
}