namespace Mural.Services;

public class WallpaperService : IWallpaperService
{
    /// <summary>
    /// Desktop wallpaper integration.
    /// </summary>
    private readonly FP.SetWallpaper.COM.IDesktopWallpaper _desktopWallpaper =
        // ReSharper disable once SuspiciousTypeConversion.Global
        (FP.SetWallpaper.COM.IDesktopWallpaper)new FP.SetWallpaper.COM.DesktopWallpaper();

    /// <summary>
    /// Indexed files.
    /// </summary>
    private readonly List<string> _files = [];

    /// <summary>
    /// Current file index.
    /// </summary>
    private int _fileIndex = -1;
    
    /// <summary>
    /// Auto-changer function thread.
    /// </summary>
    private Thread? _autoChangerThread;
    
    #region IWallpaperService functions

    /// <summary>
    /// <inheritdoc cref="IWallpaperService.GetMonitorCount"/>
    /// </summary>
    public uint? GetMonitorCount()
    {
        try
        {
            _desktopWallpaper.GetMonitorDevicePathCount(out var count);

            if (count is 0)
            {
                throw new Exception("Found 0 monitors.");
            }
            
            return count;
        }
        catch (Exception ex)
        {
            // TODO: Log exception to Windows event log.
            
            MessageBox.Show(
                "Unable to get monitor count.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return default;
        }
    }

    /// <summary>
    /// <inheritdoc cref="IWallpaperService.GetMonitorId"/>
    /// </summary>
    public string? GetMonitorId(uint index)
    {
        try
        {
            _desktopWallpaper.GetMonitorDevicePathAt(index, out var monitorId);

            if (string.IsNullOrWhiteSpace(monitorId))
            {
                throw new Exception($"Unable to find monitor at index {index}.");
            }
            
            return monitorId;
        }
        catch (Exception ex)
        {
            // TODO: Log exception to Windows event log.
            
            MessageBox.Show(
                $"Unable to get ID for monitor {index + 1}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return default;
        }
    }

    /// <summary>
    /// <inheritdoc cref="IWallpaperService.IndexFiles"/>
    /// </summary>
    public void IndexFiles()
    {
        if (Config.Values.WallpaperPath is null)
        {
            return;
        }
        
        var files = new List<string>();
        
        foreach (var filePattern in Config.Values.FilePatterns.Split(';'))
        {
            try
            {
                var temp = Directory.GetFiles(
                    Config.Values.WallpaperPath,
                    filePattern,
                    Config.Values.GetFilesRecursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                files.AddRange(temp);
            }
            catch (Exception ex)
            {
                // TODO: Log exception to Windows event log.
                
                MessageBox.Show(
                    $"Unable to get files from {Config.Values.WallpaperPath}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        if (Config.Values.ShuffleImages)
        {
            this.Shuffle(ref files);
        }
        else
        {
            files = files
                .OrderBy(n => n)
                .ToList();
        }
        
        _files.Clear();
        _files.AddRange(files);
    }

    /// <summary>
    /// <inheritdoc cref="IWallpaperService.SetWallpaper"/>
    /// </summary>
    public void SetWallpaper(string monitorId)
    {
        string? path = null;

        while (true)
        {
            if (_files.Count is 0)
            {
                break;
            }
            
            if (Config.Values.RandomWallpaper)
            {
                _fileIndex = Random.Shared.Next(0, _files.Count);
            }
            else
            {
                _fileIndex++;

                if (_fileIndex >= _files.Count)
                {
                    _fileIndex = 0;
                }
            }

            var file = _files[_fileIndex];

            if (File.Exists(file))
            {
                path = file;
                break;
            }

            _files.RemoveAt(_fileIndex);
            _fileIndex--;
        }

        if (path is null)
        {
            MessageBox.Show(
                "Unable to get file to set as wallpaper. No files found.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return;
        }

        try
        {
            _desktopWallpaper.SetWallpaper(monitorId, path);
        }
        catch (Exception ex)
        {
            // TODO: Log exception to Windows event log.
            
            MessageBox.Show(
                $"Unable to set \"{path}\" as wallpaper for monitor {monitorId}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IWallpaperService.SetWallpapers"/>
    /// </summary>
    public void SetWallpapers()
    {
        if (Config.Values.WallpaperPath is null)
        {
            MessageBox.Show(
                "No wallpaper path set. Open settings and set one.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return;
        }
        
        if (_files.Count is 0)
        {
            this.IndexFiles();
        }

        if (_files.Count is 0)
        {
            MessageBox.Show(
                "Found no files in the given wallpaper path.",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return;
        }
        
        var count = this.GetMonitorCount();

        if (count is null)
        {
            return;
        }

        for (uint i = 0; i < count; i++)
        {
            var id = this.GetMonitorId(i);
            
            if (id is null)
            {
                continue;
            }
            
            this.SetWallpaper(id);
        }
    }

    /// <summary>
    /// <inheritdoc cref="IWallpaperService.StartAutoChanger"/>
    /// </summary>
    public void StartAutoChanger()
    {
        _autoChangerThread ??= new Thread(this.AutoChangerThreadFunction);

        if (!_autoChangerThread.IsAlive)
        {
            _autoChangerThread.Start();
        }
    }

    /// <summary>
    /// <inheritdoc cref="IWallpaperService.StopAutoChanger"/>
    /// </summary>
    public void StopAutoChanger()
    {
        _autoChangerThread?.Interrupt();
    }
    
    #endregion

    #region Helper functions

    /// <summary>
    /// Change wallpaper(s) every n minutes.
    /// </summary>
    private void AutoChangerThreadFunction()
    {
        while (Config.Values.AutoChangeInterval is not null)
        {
            try
            {
                this.SetWallpapers();
                Thread.Sleep(TimeSpan.FromMinutes(Config.Values.AutoChangeInterval!.Value));
            }
            catch
            {
                break;
            }
        }
    }
    
    /// <summary>
    /// Shuffle list.
    /// </summary>
    /// <param name="list">List to shuffle.</param>
    private void Shuffle(ref List<string> list)
    {
        var rng = new Random((int)DateTime.Now.Ticks);
        var n = list.Count;

        while (n > 1)
        {
            n--;

            var k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    
    #endregion
}