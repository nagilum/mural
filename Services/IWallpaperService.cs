namespace Mural.Services;

public interface IWallpaperService
{
    /// <summary>
    /// Get number of monitors.
    /// </summary>
    /// <returns>Number of monitors.</returns>
    uint? GetMonitorCount();

    /// <summary>
    /// Get monitor ID.
    /// </summary>
    /// <param name="index">Monitor index.</param>
    /// <returns>Monitor ID.</returns>
    string? GetMonitorId(uint index);

    /// <summary>
    /// Index the wallpaper files.
    /// </summary>
    void IndexFiles();

    /// <summary>
    /// Set next file from queue as wallpaper.
    /// </summary>
    /// <param name="monitorId">Monitor ID.</param>
    void SetWallpaper(string monitorId);

    /// <summary>
    /// Set new wallpapers on all monitors from queue.
    /// </summary>
    void SetWallpapers();

    /// <summary>
    /// Start the auto-change-wallpaper thread.
    /// </summary>
    void StartAutoChanger();

    /// <summary>
    /// Stop the auto-change-wallpaper thread.
    /// </summary>
    void StopAutoChanger();
}