namespace Snailer.GodotCSharp.SettingsManager.Tests;

using NUnit.Framework;
using Snailer.GodotCSharp.SettingsManager.Tests.Data;

[TestFixture]
public class SettingsManagerTests
{
    private const string SETTINGS_NAME = "test.json";

    [TearDown]
    public void TearDown()
    {
        // Delete generated .json file
        var path = JsonFileHelper.GetPathToFile(SETTINGS_NAME);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    [Test]
    public void SetSetting_WithComplexTypes_SetsComplexTypeValue()
    {
        // Arrange
        var mgr = new SettingsManager<SettingsWithComplexTypes>(SETTINGS_NAME);
        var settings = mgr.GetSettings();
        var firstComplexPropertyOriginalValue = settings.FirstNestedClass.PropertyInFirstNestedClass;
        var secondComplexPropertyOriginalValue = settings.FirstNestedClass.SecondNestedClass.PropertyInSecondNestedClass;
        var expectedStringList = new string[] { "a", "b" };
        var expectedFirstComplexPropertyValue = firstComplexPropertyOriginalValue + 1;
        var expectedSecondComplexPropertyValue = secondComplexPropertyOriginalValue + 2;

        // Act
        mgr.SetSetting(nameof(SettingsWithComplexTypes.StringList), expectedStringList);
        mgr.SetSetting(nameof(SettingsWithComplexTypes.FirstNestedClass.PropertyInFirstNestedClass), expectedFirstComplexPropertyValue);
        mgr.SetSetting(nameof(SettingsWithComplexTypes.FirstNestedClass.SecondNestedClass.PropertyInSecondNestedClass), expectedSecondComplexPropertyValue);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(settings.FirstNestedClass.PropertyInFirstNestedClass, Is.EqualTo(expectedFirstComplexPropertyValue));
            Assert.That(settings.FirstNestedClass.SecondNestedClass.PropertyInSecondNestedClass, Is.EqualTo(expectedSecondComplexPropertyValue));
            Assert.That(settings.StringList, Is.EqualTo(expectedStringList));
        });
    }

    [Test]
    public void SetSetting_WithSimpleTypes_SetsSimpleTypeValue()
    {
        // Arrange
        var mgr = new SettingsManager<SettingsWithSimpleTypes>(SETTINGS_NAME);
        var settings = mgr.GetSettings();
        var expectedIntegerPropertyValue = 42;
        var expectedStringPropertyValue = "a";
        var expectedDecimalPropertyValue = 0.5M;
        var expectedBoolPropertyValue = true;

        // Act
        mgr.SetSetting(nameof(SettingsWithSimpleTypes.IntProperty), expectedIntegerPropertyValue);
        mgr.SetSetting(nameof(SettingsWithSimpleTypes.StringProperty), expectedStringPropertyValue);
        mgr.SetSetting(nameof(SettingsWithSimpleTypes.DecimalProperty), expectedDecimalPropertyValue);
        mgr.SetSetting(nameof(SettingsWithSimpleTypes.BoolProperty), expectedBoolPropertyValue);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(settings.IntProperty, Is.EqualTo(expectedIntegerPropertyValue));
            Assert.That(settings.StringProperty, Is.EqualTo(expectedStringPropertyValue));
            Assert.That(settings.DecimalProperty, Is.EqualTo(expectedDecimalPropertyValue));
            Assert.That(settings.BoolProperty, Is.EqualTo(expectedBoolPropertyValue));
        });
    }

    [Test]
    public void AutosaveOff_Load_ResetsValues()
    {
        // Arrange
        var mgr = new SettingsManager<SettingsWithDefaultValues>(SETTINGS_NAME, new() { AutoSaveChanges = false });
        var settings = mgr.GetSettings();
        var defaultStringValue = settings.StringProperty;
        var defaultIntValue = settings.IntProperty;
        var defaultBoolValue = settings.BoolProperty;

        // Act
        mgr.SetSetting(nameof(SettingsWithDefaultValues.StringProperty), defaultStringValue + "test");
        mgr.SetSetting(nameof(SettingsWithDefaultValues.IntProperty), defaultIntValue + 1);
        mgr.SetSetting(nameof(SettingsWithDefaultValues.BoolProperty), !defaultBoolValue);
        mgr.Load();
        settings = mgr.GetSettings();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(settings.StringProperty, Is.EqualTo(defaultStringValue));
            Assert.That(settings.IntProperty, Is.EqualTo(defaultIntValue));
            Assert.That(settings.BoolProperty, Is.EqualTo(defaultBoolValue));
        });
    }

    [Test]
    public void AutosaveOff_Save_CommitsValues()
    {
        // Arrange
        var mgr = new SettingsManager<SettingsWithSimpleTypes>(SETTINGS_NAME, new() { AutoSaveChanges = false });
        var expectedStringValue = "test";

        // Act
        mgr.SetSetting(nameof(SettingsWithDefaultValues.StringProperty), expectedStringValue);
        mgr.Save();
        mgr.Load();
        var settings = mgr.GetSettings();

        // Assert
        Assert.That(settings.StringProperty, Is.EqualTo(expectedStringValue));
    }

    [Test]
    public void AutosaveOn_SetDefaults_ResetsValuesAndCommits()
    {
        // Arrange
        var mgr = new SettingsManager<SettingsWithDefaultValues>(SETTINGS_NAME);
        var settings = mgr.GetSettings();
        var defaultStringValue = settings.StringProperty;

        // Act
        mgr.SetSetting(nameof(SettingsWithDefaultValues.StringProperty), defaultStringValue + "test");
        mgr.SetDefaults();
        var inMemorySettings = mgr.GetSettings();
        mgr.Load();
        var settingsFromFile = mgr.GetSettings();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(inMemorySettings.StringProperty, Is.EqualTo(defaultStringValue));
            Assert.That(settingsFromFile.StringProperty, Is.EqualTo(defaultStringValue));
        });
    }

    [Test]
    public void AutosaveOff_SetDefaults_ResetsValuesDoesntCommit()
    {
        // Arrange
        var mgr = new SettingsManager<SettingsWithDefaultValues>(SETTINGS_NAME, new() { AutoSaveChanges = false });
        var settings = mgr.GetSettings();
        var defaultStringValue = settings.StringProperty;
        var modifiedStringValue = defaultStringValue + "test";

        // Act
        mgr.SetSetting(nameof(SettingsWithDefaultValues.StringProperty), modifiedStringValue);
        var settingsBeforeReset = mgr.GetSettings();
        mgr.SetDefaults();
        var settingsAfterReset = mgr.GetSettings();
        mgr.Load();
        var settingsAfterLoad = mgr.GetSettings();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(settingsBeforeReset.StringProperty, Is.EqualTo(modifiedStringValue));
            Assert.That(settingsAfterReset.StringProperty, Is.EqualTo(defaultStringValue));
            Assert.That(settingsAfterLoad.StringProperty, Is.EqualTo(defaultStringValue));
        });
    }
}