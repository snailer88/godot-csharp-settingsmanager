namespace Snailer.GodotCSharp.SettingsManager;

using System.Reflection;

public static class DirectoryHelper
{
    public static string ExecutingAssemblyDirectory
    {
        get
        {
            var location = Assembly.GetExecutingAssembly().Location;

            return Path.GetDirectoryName(location) ?? throw new InvalidOperationException("Failed to locate executing directory.");
        }
    }
}
