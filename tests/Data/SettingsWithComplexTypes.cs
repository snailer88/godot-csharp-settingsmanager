namespace Snailer.GodotCSharp.SettingsManager.Tests.Data;

public class SettingsWithComplexTypes : ISettings
{
    public FirstNestedClass FirstNestedClass { get; set; } = new();

    public IEnumerable<string> StringList { get; set; } = [];

    public void InitializeSettings()
    {
    }

    public void HandleSettingChange(string settingName, object value)
    {
    }
}

public class FirstNestedClass
{
    public int PropertyInFirstNestedClass { get; set; } = 10;

    public SecondNestedClass SecondNestedClass { get; set; } = new();
}

public class SecondNestedClass
{
    public int PropertyInSecondNestedClass { get; set; } = 20;
}