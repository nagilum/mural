using System.Text;
using System.Text.Json;
using Mural.Models;

namespace Mural;

public static class Config
{
    /// <summary>
    /// Full path to config file.
    /// </summary>
    private static readonly string FilePath = Path.Combine(Application.StartupPath, "config.json");
    
    /// <summary>
    /// Loaded config values.
    /// </summary>
    public static IConfigValues Values { get; private set; } = new ConfigValues();
    
    /// <summary>
    /// Load config from disk.
    /// </summary>
    public static void Load()
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                return;
            }

            var json = File.ReadAllText(FilePath, Encoding.UTF8);
            var values = JsonSerializer.Deserialize<ConfigValues>(json)
                         ?? throw new Exception("Unable to deserialize loaded config.");

            Values = values;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to load config from {FilePath}{Environment.NewLine}{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Save config to disk.
    /// </summary>
    public static void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(Values);

            File.WriteAllText(FilePath, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to load config from {FilePath}{Environment.NewLine}{ex.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}