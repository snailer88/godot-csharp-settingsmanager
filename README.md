# Godot Settings Manager

[![NuGet](https://img.shields.io/nuget/v/Snailer.GodotCSharp.SettingsManager)](https://www.nuget.org/packages/Snailer.GodotCSharp.SettingsManager#versions-body-tab)
[![build](https://github.com/snailer88/godot-csharp-settingsmanager/actions/workflows/build.yml/badge.svg)](https://github.com/snailer88/godot-csharp-settingsmanager/actions/workflows/build.yml)

The `SettingsManager` can be used to read/write local JSON settings files when developing [Godot C# projects](https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_basics.html). Multiple settings files can be managed by creating as many `SettingsManager` instances as needed, utilizing different paths and settings classes. For example, you can create a manager for audio and video settings:

```cs
var audioSettingsManager = new SettingsManager<AudioSettings>("audio.json");
var videoSettingsManager = new SettingsManager<VideoSettings>("video.json");
```

## Installation

Add the NuGet package to your Godot project:

```bash
dotnet add package Snailer.GodotCSharp.SettingsManager
```

## Getting started

Create a class implementing `ISettings` which contains properties related to your program's settings.

```cs
public class MyGameSettings : ISettings
{
  public const string FILENAME = "mysettings.json";

  public bool IsFullscreen { get; set; } = true;
}
```

During your program's startup, create a new `SettingsManager` which will be responsible for loading/saving your settings to a local JSON file.

> :bulb: You may want to store the manager in some static class to be accessed later!

```cs
GlobalObjects.MySettingsManager = new SettingsManager<MyGameSettings>(MyGameSettings.FILENAME);
```

## Documentation

Check out the documentation for detailed usage scenarios and examples!

- [Creating a settings class](/docs/CreatingSettings.md)
- [Managing settings](/docs/ManagingSettings.md)
