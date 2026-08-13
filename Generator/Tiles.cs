using System;
using System.IO;
using System.Linq;

namespace Generator;

public sealed class Tiles : IGenerator
{
    public string GeneratedSource => "Tiles.cs";
    public string[] DirectoriesWhitelist => ["Images/Tiles", "Images/Objects"];
    public string[] IgnoredFiles => [];

    public void Generate(string root)
    {
        string[] directories = DirectoriesWhitelist.Select(d => $"{root}/{d}").ToArray();

        string output = $"{Environment.CurrentDirectory}/Utils/{GeneratedSource}";
        using (CodeWriter writer = new CodeWriter(output))
        {
            writer.WriteLine("""
                             using SFDGameScriptInterface;

                             namespace SFD.Scripting.Resources;

                             public partial class GameScript : GameScriptInterfaceExtended
                             {
                             """);
            writer.IncrementPadding();

            writer.WriteLine("""
                             public static class TilesDatabase
                             {
                             """);
            writer.IncrementPadding();

            foreach (string file in Directory.GetFiles(directories, "*.png", SearchOption.AllDirectories))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (Enumerable.Contains(IgnoredFiles, fileName)) continue;
                writer.WriteLine($"public const string {fileName.Replace("_", string.Empty)} = \"{fileName}\";");
            }

            writer.DecrementPadding();
            writer.WriteLine("}");

            writer.DecrementPadding();
            writer.WriteLine("}");
        }
    }
}