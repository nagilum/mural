namespace Mural.Models;

public interface IConfigValues
{
    /// <summary>
    /// Automatically change wallpaper every n minutes.
    /// </summary>
    double? AutoChangeInterval { get; set; }
    
    /// <summary>
    /// Automatically start Mural when logging into Windows.
    /// </summary>
    bool AutoStart { get; set; }
    
    /// <summary>
    /// Which file extensions to look for, semicolon separated.
    /// </summary>
    string FilePatterns { get; set; }
    
    /// <summary>
    /// Get files recursively.
    /// </summary>
    bool GetFilesRecursively { get; set; }
    
    /// <summary>
    /// Get a random wallpaper each time instead of following an alphabetic order.
    /// </summary>
    bool RandomWallpaper { get; set; }
    
    /// <summary>
    /// Shuffle images.
    /// </summary>
    bool ShuffleImages { get; set; }
    
    /// <summary>
    /// Path to wallpapers.
    /// </summary>
    string? WallpaperPath { get; set; }
}