namespace Snailer.GodotCSharp.SettingsManager.Tests.Data;

public class SettingsWithSimpleTypes : ISettings
{
    public int IntProperty { get; set; }

    public string? StringProperty { get; set; }

    public bool BoolProperty { get; set; }

    public decimal DecimalProperty { get; set; }

    public void InitializeSettings()
    {
    }

    public void HandleSettingChange(string settingName, object value)
    {
    }
}
