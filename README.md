[![NuGet](https://img.shields.io/nuget/v/Snailer.GodotCSharp.SettingsManager)](https://www.nuget.org/packages/Snailer.GodotCSharp.SettingsManager#versions-body-tab)
[![build](https://github.com/snailer88/godot-csharp-settingsmanager/actions/workflows/build.yml/badge.svg)](https://github.com/snailer88/godot-csharp-settingsmanager/actions/workflows/build.yml)

# Godot Settings Manager

The `SettingsManager` can be used to read/write local JSON settings files when developing [Godot C# projects](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_basics.html). Multiple settings files can be managed by creating as many `SettingsManager` instances as needed, utilizing different paths and settings classes. For example, you can create a manager for audio and video settings:

```cs
var audioSettingsManager = new SettingsManager<AudioSettings>("audio.json");
var videoSettingsManager = new SettingsManager<VideoSettings>("video.json");
```

You can also opt to contain all settings within a single settings file. Using nested classes, you can still organize categories of settings:

```cs
public class GameSettings : ISettings
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

## Installation

Add the NuGet package to your Godot project:

```bash
dotnet add package Snailer.GodotCSharp.SettingsManager
```

## Creating a settings class

Your game's settings will be represented by a class you create which implements `ISetting`. The class should contain properties which correspond with the settings you wish to manage in your project. In this example, the settings file contains a single "IsFullscreen" setting with a default value:

```cs
public class GameSettings : ISettings
{
  public const string FILENAME = "settings.json";

  public bool IsFullscreen { get; set; } = true;
}
```

Resulting .json file:
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

### Autosaving

By default, autosaving is enabled. This means that when `SetSettings()` is called, the JSON file is automatically written to. You may wish to instead provide a "Cancel" and "Save" button in your settings menu, allowing users to discard any changes made to the settings.

```cs
var mgr = new SettingsManager<GameSettings>(GameSettings.FILENAME, new() { AutoSaveChanges = false });
```

With autosave disabled, you must handle the saving of modified settings, and reload saved settings from the JSON file in the case that the user cancels the changes. You can do so with the `Save()` and `Load()` methods, respectively:

```cs
public void OnSaveSettingsClicked() => mgr.Save();

public void OnCancelSettingsClicked() => mgr.Load();
```

### Auto-handing changes

By default, when a setting is modified via `SetSetting()`, the `ISetting.HandleSettingChange()` method is called allowing you to react to the settings change and apply it to the program. You could disable this behavior, for example if your "Settings" scene has an "Apply" button.

```cs
var mgr = new SettingsManager<GameSettings>(GameSettings.FILENAME, new() { AutoHandleChanges = false });
```

When disabled, you can manually call `ISettings.HandleSettingChange` to initialize a single setting, or `ISettings.InitializeSettings` to initialize all settings.

### Setting defaults

Your settings menu may contain a "Defaults" button which removes any user changes and loads your default settings. You can use the `SetDefaults()` method to accomplish this:

```cs
public void OnDefaultsSettingsClicked() => mgr.SetDefaults();
```

If autosaving is enabled, the default settings are then written to the JSON file. Otherwise, you can use `Save()` or `Load()` to save the defaults, or return the settings to the previous user-specified settings.
