# Creating a settings class

Your program's settings will be represented by a class you create which implements `ISetting`. The class should contain properties which correspond with the settings you wish to manage in your project. In this example, the settings file contains a single "IsFullscreen" setting with a default value.

```cs
public class MyGameSettings : ISettings
{
  public bool IsFullscreen { get; set; } = true;
}
```

You must also implement the methods which set the project's parameters on load, and react to settings changes:

```cs
public class MyGameSettings : ISettings
{
  public bool IsFullscreen { get; set; } = true;

  // Called at program start or when reloaded
  public void InitializeSettings()
  {
    SetFullscreen(IsFullscreen);
  }

  // Called when user changes setting
  public void HandleSettingChange(string settingName, object value)
  {
    switch (settingName)
    {
      case nameof(IsFullscreen):
        SetFullscreen((bool)value);
        break;
      default:
        break;
    }
  }

  private static void SetFullscreen(bool isFullscreen) =>
    DisplayServer.WindowSetMode(isFullscreen ? DisplayServer.WindowMode.Fullscreen : DisplayServer.WindowMode.Windowed);
}
```

Instead of creating multiple `ISettings` classes to manage your settings, you can also opt to contain all settings within a single settings file. Using nested classes, you can still organize categories of settings:

```cs
public class MyGameSettings : ISettings
{
    public Audio Audio { get; set; } = new();

    public Video Video { get; set; } = new();
}

public class Audio
{
    public double MusicVolume { get; set; } = 0.5;
}

public class Video
{
    public bool IsFullscreen { get; set; } = true;
}
```