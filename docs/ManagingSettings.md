# Managing settings

After you have created your `ISettings` class(es), you should instantiate a new `SettingsManager` at program start. Creating a new `SettingsManager` automatically creates the JSON file (if not present) and calls the `InitializeSettings()` method of your `ISettings` class to restore the settings from the JSON file:

```cs
var mgr = new SettingsManager<MyGameSettings>("mysettings.json");
```



Now, if you need to access the settings within the project (e.g. in your "Settings" scene), call `GetSettings()`:

```cs
MyGameSettings? settings = mgr.GetSettings();
var isFullscreen = settings?.IsFullscreen;
```

To update the value of a setting in both the `ISettings` object and JSON file, simply call `SetSetting()`. This will call your `HandleSettingChange()` method, allowing you to react to the new value. For example, this could be the full click handler for a fullscreen toggle in your "Settings" scene:

```cs
public void OnFullscreenToggled(bool enabled) => mgr.SetSetting(nameof(MyGameSettings.IsFullscreen), enabled);
```

## SettingsManager options

When creating a new `SettingsManager`, you can provide options in the constructor which control how the manager behaves.

- [Autosaving](#autosaving)
- [Auto-handling changes](#auto-handing-changes)

### Autosaving

By default, autosaving is enabled. This means that when `SetSetting()` is called, the JSON file is automatically written to. You may wish to instead provide a "Cancel" and "Save" button in your settings menu, allowing users to discard any changes made to the settings.

```cs
var mgr = new SettingsManager<MyGameSettings>("mysettings.json", new() { AutoSaveChanges = false });
```

With autosave disabled, you must handle the saving of modified settings, and reload saved settings from the JSON file in the case that the user cancels the changes. You can do so with the `Save()` and `Load()` methods, respectively:

```cs
public void OnSaveSettingsClicked() => mgr.Save();

public void OnCancelSettingsClicked() => mgr.Load();
```

### Auto-handing changes

By default, when a setting is modified via `SetSetting()`, the `ISetting.HandleSettingChange()` method is called allowing you to react to the settings change and apply it to the program. You could disable this behavior, for example if your "Settings" scene has an "Apply" button.

```cs
var mgr = new SettingsManager<MyGameSettings>("mysettings.json", new() { AutoHandleChanges = false });
```

When disabled, you can manually call `ISettings.HandleSettingChange` to initialize a single setting, or `ISettings.InitializeSettings` to initialize all settings.

## Setting defaults

Your settings menu may contain a "Defaults" button which removes any user changes and loads your default settings. You can use the `SetDefaults()` method to accomplish this:

```cs
public void OnDefaultsSettingsClicked() => mgr.SetDefaults();
```

If autosaving is enabled, the default settings are then written to the JSON file. Otherwise, you can use `Save()` or `Load()` to save the defaults, or return the settings to the previous user-specified settings.