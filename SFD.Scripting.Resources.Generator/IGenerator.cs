namespace SFD.Scripting.Resources.Generator;

public interface IGenerator
{
    public string GeneratedSource { get; }
    public string[] DirectoriesWhitelist { get; }
    public string[] IgnoredFiles { get; }

    public void Generate(string dataRoot, string outputDir);
}
