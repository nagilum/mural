using System.Text;
using System.Text.Json;
using Mural.Models;

namespace Mural;

public static class Config
{
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
            var path = Path.Combine(Application.StartupPath, "config.json");

            if (!File.Exists(path))
            {
                return;
            }

            var json = File.ReadAllText(path, Encoding.UTF8);
            var values = JsonSerializer.Deserialize<ConfigValues>(json)
                         ?? throw new Exception("Unable to deserialize loaded config.");

            Values = values;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
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
            var path = Path.Combine(Application.StartupPath, "config.json");
            var json = JsonSerializer.Serialize(Values);

            File.WriteAllText(path, json, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}