namespace Snailer.GodotCSharp.SettingsManager;

using System;

/// <summary>
/// Contains methods for loading and saving settings files.
/// </summary>
public class SettingsManager<T> where T : class, ISettings
{
  private T? _settings;
  private readonly bool _autosave;
  private readonly string _fileName;

  /// <summary>
  /// Instantiates a new manager for reading/writing to the provided <paramref name="fileName"/>.
  /// </summary>
  /// <param name="fileName">The name of the JSON file settings are stored in.</param>
  /// <param name="autosave">If <c>false</c>, the JSON file is only written to when <see cref="Save"/> is called.</param>
  public SettingsManager(string fileName, bool autosave = true)
  {
    _fileName = fileName;
    _autosave = autosave;
    JsonFileHelper.EnsureJsonFile<T>(_fileName);
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

    return JsonFileHelper.ReadJsonFile<T>(_fileName);
  }

  /// <summary>
  /// Loads the settings from the JSON file into the in-memory object, and initializes the settings.
  /// </summary>
  public void Load()
  {
    _settings = JsonFileHelper.ReadJsonFile<T>(_fileName);
    _settings.InitializeSettings();
  }

  /// <summary>
  /// Writes the in-memory settings object to the JSON file.
  /// </summary>
  public void Save() => JsonFileHelper.WriteJsonFile(_settings, _fileName);

  /// <summary>
  /// Sets the value of an in-memory setting. If autosave is enabled, the JSON file is also updated.
  /// </summary>
  /// <param name="settingName">The name of the setting to update.</param>
  /// <param name="value">The new value of the setting.</param>
  public bool SetSetting(string settingName, object value)
  {
    ArgumentNullException.ThrowIfNull(_settings);

    // Set property on settings object
    var expandedPropName = string.Join('.', GetFullPropertyPath(typeof(T), settingName));
    var didSet = SetProperty(expandedPropName, _settings, value);
    if (!didSet)
    {
      return false;
    }

    _settings.HandleSettingChange(settingName, value);

    if (_autosave)
    {
      Save();
    }

    return true;
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
  /// Sets a property value on the target object.
  /// </summary>
  /// <param name="compoundProperty">The full path of the property where nested classes are separated with periods, e.g.
  /// "MyClass.NestedClass.PropName"</param>
  /// <param name="target">The object to set the value of.</param>
  /// <param name="value">The value to set on the property.</param>
  /// <returns><c>True</c> if the property was successfully set.</returns>
  private static bool SetProperty(string compoundProperty, object? target, object value)
  {
    var bits = compoundProperty.Split('.');
    bits ??= new string[1] { compoundProperty };
    for (var i = 0; i < bits.Length - 1; i++)
    {
      var propertyToGet = target?.GetType().GetProperty(bits[i]);
      var propertyValue = propertyToGet?.GetValue(target, null);
      if (propertyValue is null && propertyToGet is not null)
      {
        propertyValue = Activator.CreateInstance(propertyToGet.PropertyType);
        propertyToGet.SetValue(target, propertyValue);
      }

      target = propertyToGet?.GetValue(target, null);
    }

    var propertyToSet = target?.GetType().GetProperty(bits.Last());
    if (propertyToSet is not null)
    {
      propertyToSet.SetValue(target, value, null);

      return true;
    }

    return false;
  }

  /// <summary>
  /// Returns the full path of a property within the given type, recursively to include properties which are classes.
  /// </summary>
  private static IEnumerable<string> GetFullPropertyPath(Type baseType, string propertyName)
  {
    var prop = baseType.GetProperty(propertyName);
    if (prop != null)
    {
      return new[] { prop.Name };
    }
    else if (baseType.IsClass && baseType != typeof(string)) // Do not go into primitives (condition could be refined, this excludes all structs and strings)
    {
      return baseType
          .GetProperties()
          .SelectMany(p => GetFullPropertyPath(p.PropertyType, propertyName), (p, v) => p.Name + "." + v);
    }
    return Enumerable.Empty<string>();
  }
}
