namespace Snailer.GodotCSharp.SettingsManager;

using System.Reflection;

public static class DirectoryHelper
{
    /// <summary>
    /// Gets the full path of a file within the executing directory.
    /// </summary>
    public static string GetPathToFile(string fileName)
    {
        var executingDir = Assembly.GetExecutingAssembly().Location;
        if (executingDir.EndsWith(".dll"))
        {
            executingDir = Path.GetDirectoryName(executingDir);
        }

        if (executingDir is null)
        {
            throw new InvalidOperationException("Unable to load executing directory.");
        }

        return Path.Combine(executingDir, fileName);
    }
}
