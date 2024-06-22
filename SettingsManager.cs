namespace Snailer.GodotCSharp.SettingsManager;

using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

/// <summary>
/// Contains methods for loading and saving the settings file.
/// </summary>
public class SettingsManager<T> where T : class, ISettings
{
  private readonly string _fileName;
  private readonly T _settings;

  public SettingsManager(string fileName)
  {
    _fileName = fileName;
    EnsureSettings();
    _settings = GetSettings();
    _settings.InitializeSettings();
  }

  public T GetSettings()
  {
    if (_settings is not null)
    {
      return _settings;
    }

    var path = GetSettingsPath();
    if (!File.Exists(path))
    {
      throw new FileNotFoundException($"The settings file '{path}' was not found.");
    }

    var text = File.ReadAllText(path);

    return JsonConvert.DeserializeObject<T>(text)
      ?? throw new JsonReaderException($"The settings file '{path}' cannot be deserialized.");
  }

  public void SetSetting(string settingName, object value)
  {
    ArgumentNullException.ThrowIfNull(_settings);

    // Set property on settings object
    var settingsProp = _settings.GetType().GetProperty(settingName);
    ArgumentNullException.ThrowIfNull(settingsProp);
    settingsProp.SetValue(_settings, value);

    _settings.HandleSettingChange(settingName, value);
    WriteSettings(_settings);
    Console.WriteLine($"Setting '{settingName}' changed to '{value}'");
  }

  private void EnsureSettings()
  {
    if (File.Exists(GetSettingsPath()))
    {
      return;
    }

    WriteSettings(Activator.CreateInstance(typeof(T)));
  }

  private void WriteSettings(object? settings) => File.WriteAllText(GetSettingsPath(), JsonConvert.SerializeObject(settings, Formatting.Indented));

  private string GetSettingsPath() => Path.Combine(Assembly.GetExecutingAssembly().Location, _fileName);
}
