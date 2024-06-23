namespace Snailer.GodotCSharp.SettingsManager;

using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

/// <summary>
/// Contains methods for loading and saving settings files.
/// </summary>
public class SettingsManager<T> where T : class, ISettings
{
  private readonly bool _autosave;
  private readonly string _fileName;
  private T? _settings;

  /// <summary>
  /// Instantiates a new manager for reading/writing to the provided <paramref name="fileName"/>.
  /// </summary>
  /// <param name="fileName">The name of the JSON file settings are stored in.</param>
  /// <param name="autosave">If <c>false</c>, the JSON file is only written to when <see cref="Save"/> is called.</param>
  public SettingsManager(string fileName, bool autosave = true)
  {
    _fileName = fileName;
    _autosave = autosave;
    Ensure();
    Load();
  }

  /// <summary>
  /// Gets the in-memory settings object, or loads the settings from the filesystem if not initialized.
  /// </summary>
  public T GetSettings()
  {
    if (_settings is not null)
    {
      return _settings;
    }

    return ReadSettings();
  }

  /// <summary>
  /// Loads the settings from the JSON file into the in-memory object, and initializes the settings.
  /// </summary>
  public void Load()
  {
    _settings = ReadSettings();
    _settings.InitializeSettings();
  }

  /// <summary>
  /// Writes the in-memory settings object to the JSON file.
  /// </summary>
  public void Save() => WriteSettings(_settings);

  /// <summary>
  /// Sets the value of an in-memory setting. If autosave is enabled, the JSON file is also updated.
  /// </summary>
  /// <param name="settingName">The name of the setting to update.</param>
  /// <param name="value">The new value of the setting.</param>
  public void SetSetting(string settingName, object value)
  {
    ArgumentNullException.ThrowIfNull(_settings);

    // Set property on settings object
    var settingsProp = _settings.GetType().GetProperty(settingName);
    if (settingsProp is null)
    {
      return;
    }
    settingsProp.SetValue(_settings, value);
    _settings.HandleSettingChange(settingName, value);

    if (_autosave)
    {
      Save();
    }
  }

  /// <summary>
  /// Discards the in-memory settings object, loads the default values, and re-initializes the settings.
  /// </summary>
  public void SetDefaults()
  {
    _settings = Activator.CreateInstance<T>();
    _settings.InitializeSettings();
    if (_autosave)
    {
      Save();
    }
  }

  /// <summary>
  /// Ensures that the settings JSON file exists. If not, it is created with the default settings.
  /// </summary>
  private void Ensure()
  {
    if (File.Exists(GetSettingsPath()))
    {
      return;
    }

    WriteSettings(Activator.CreateInstance<T>());
  }

  /// <summary>
  /// Writes the provided <paramref name="settings"/> object to the JSON file.
  /// </summary>
  private void WriteSettings(T? settings) => File.WriteAllText(GetSettingsPath(), JsonConvert.SerializeObject(settings, Formatting.Indented));

  /// <summary>
  /// Gets the absolute path to the settings JSON file.
  /// </summary>
  /// <returns></returns>
  private string GetSettingsPath() => Path.Combine(Assembly.GetExecutingAssembly().Location, _fileName);

  /// <summary>
  /// Reads the JSON file from the filesystem and returns the settings object.
  /// </summary>
  /// <exception cref="FileNotFoundException"></exception>
  /// <exception cref="JsonReaderException"></exception>
  private T ReadSettings()
  {
    var path = GetSettingsPath();
    if (!File.Exists(path))
    {
      throw new FileNotFoundException($"The settings file '{path}' was not found.");
    }

    var text = File.ReadAllText(path);

    return JsonConvert.DeserializeObject<T>(text)
      ?? throw new JsonReaderException($"The settings file '{path}' cannot be deserialized.");
  }
}
