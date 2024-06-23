[![NuGet](https://img.shields.io/nuget/v/Snailer.GodotCSharp.SettingsManager)](https://www.nuget.org/packages/Snailer.GodotCSharp.SettingsManager#versions-body-tab)
[![build](https://github.com/snailer88/godot-csharp-settingsmanager/actions/workflows/build.yml/badge.svg)](https://github.com/snailer88/godot-csharp-settingsmanager/actions/workflows/build.yml)

# Godot Settings Manager

The `SettingsManager` can be used to read/write from local JSON settings files when developing [Godot C# projects](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_basics.html).

## Installation

Add the NuGet package to your Godot project:

```bash
dotnet add package Snailer.GodotCSharp.SettingsManager
```

## Creating a settings class

Your game's settings will be represented by a class you create which implements `ISetting`. The class should contain properties which correspond with the settings you wish to manage in your project. In this example, the settings file contains a single "IsFullscreen" setting with a default value:

--__GameSettings.cs__--
```cs
public class GameSettings : ISettings
{
  public const string FILENAME = "settings.json";

  public bool IsFullscreen { get; set; } = true;
}
```

--__Generated .json file__--
```json
{
  "IsFullscreen": true
}
```

You must also implement the methods which set the project's parameters on load, and react to settings changes:

```cs
public class GameSettings : ISettings
{
  public const string FILENAME = "settings.json";

  public bool IsFullscreen { get; set; } = true;

  // Called at program start
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

## Loading and saving your settings

Now that you have your settings class created, you should instantiate a new `SettingsManager` at program start. Creating a new `SettingsManager` automatically creates the JSON file (if not present) and calls the `InitializeSettings()` method of your `ISettings` class to restore the settings from the JSON file:

```cs
var mgr = new SettingsManager<GameSettings>(GameSettings.FILENAME);
```

> :bulb: You may want to store the loaded settings in some static class to be accessed later!

Now, if you need to access the settings within the project (e.g. in your "Settings" scene), call `GetSettings()`:

```cs
GameSettings? settings = mgr.GetSettings();
var isFullscreen = settings?.IsFullscreen;
```

To update the value of a setting in both the `ISettings` object and JSON file, simply call `SetSetting()`. This will call your `HandleSettingChange()` method, allowing you to react to the new value. For example, this could be the full click handler for a fullscreen toggle in your "Settings" scene:

```cs
public void OnFullscreenToggled(bool toggled) => mgr.SetSetting(nameof(GameSettings.IsFullscreen), toggled);
```
