using System.Reflection;
using Microsoft.Win32;
using Mural.Forms;

namespace Mural;

internal static class Program
{
    /// <summary>
    /// Program name.
    /// </summary>
    public const string Name = "Mural";

    /// <summary>
    /// Program version.
    /// </summary>
    public const string Version = "0.1-alpha";
    
    /// <summary>
    /// Application icon.
    /// </summary>
    public static Icon? ApplicationIcon { get; } = GetApplicationIcon();
    
    /// <summary>
    /// Init all the things...
    /// </summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new TrayForm());

        /*
        var dw = (FP.SetWallpaper.COM.IDesktopWallpaper)new FP.SetWallpaper.COM.DesktopWallpaper();
        var dict = new Dictionary<string, string?>();

        dw.GetMonitorDevicePathCount(out var count);

        for (uint i = 0; i < count; i++)
        {
            dw.GetMonitorDevicePathAt(i, out var id);
            dw.GetWallpaper(id, out var path);

            dict.Add(id, path);
        }
        */
    }
    
    /// <summary>
    /// Attempt to get application icon from the executable.
    /// </summary>
    /// <returns>Application icon.</returns>
    private static Icon? GetApplicationIcon()
    {
        try
        {
            var assembly = Assembly.GetEntryAssembly()
                           ?? throw new Exception("Unable to get entry assembly.");
            
            return Icon.ExtractAssociatedIcon(assembly.Location);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Returns whether Mural is set to auto-start via registry.
    /// </summary>
    public static bool IsSetToAutoStart()
    {
        try
        {
            using var regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)
                               ?? throw new Exception("Unable to open registry key.");

            return regKey.GetValue(Name) is not null;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

            return false;
        }
    }

    /// <summary>
    /// Set whether to automatically start Mural when logging into Windows.
    /// </summary>
    public static void SetProgramAutoStart(bool state)
    {
        try
        {
            using var regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true)
                               ?? throw new Exception("Unable to open registry key.");

            if (state)
            {
                regKey.SetValue(Name, Application.ExecutablePath);
            }
            else
            {
                regKey.DeleteValue(Name, false);
            }
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