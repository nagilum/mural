namespace Mural.Models;

public class ConfigValues : IConfigValues
{
    /// <summary>
    /// <inheritdoc cref="IConfigValues.AutoChangeInterval"/>
    /// </summary>
    public double? AutoChangeInterval { get; set; }

    /// <summary>
    /// <inheritdoc cref="IConfigValues.AutoStart"/>
    /// </summary>
    public bool AutoStart { get; set; }

    /// <summary>
    /// <inheritdoc cref="IConfigValues.FilePatterns"/>
    /// </summary>
    public string FilePatterns { get; set; } = "*";

    /// <summary>
    /// <inheritdoc cref="IConfigValues.GetFilesRecursively"/>
    /// </summary>
    public bool GetFilesRecursively { get; set; }

    /// <summary>
    /// <inheritdoc cref="IConfigValues.RandomWallpaper"/>
    /// </summary>
    public bool RandomWallpaper { get; set; }

    /// <summary>
    /// <inheritdoc cref="IConfigValues.ShuffleImages"/>
    /// </summary>
    public bool ShuffleImages { get; set; }

    /// <summary>
    /// <inheritdoc cref="IConfigValues.WallpaperPath"/>
    /// </summary>
    public string? WallpaperPath { get; set; }
}