namespace Snailer.GodotCSharp.SettingsManager.Tests.Data;

public class SettingsWithDefaultValues : ISettings
{
    public int IntProperty { get; set; } = 1;

    public string StringProperty { get; set; } = "a";

    public bool BoolProperty { get; set; } = true;

    public void InitializeSettings()
    {
    }

    public void HandleSettingChange(string settingName, object value)
    {
    }
}
