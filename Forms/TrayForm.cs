using Mural.Dialogs;
using Mural.Services;

namespace Mural.Forms;

public class TrayForm : Form
{
    /// <summary>
    /// Wallpaper service.
    /// </summary>
    private readonly WallpaperService _service = new();
    
    /// <summary>
    /// Set up the tray form.
    /// </summary>
    public TrayForm()
    {
        this.SetupTrayMenu();
        this.SetupForm();
        
        Config.Load();
        
        _service.IndexFiles();

        if (Config.Values.AutoChangeInterval is not null)
        {
            _service.StartAutoChanger();
        }
    }

    /// <summary>
    /// Add tray menu and events.
    /// </summary>
    private void SetupTrayMenu()
    {
        var notifyIcon = new NotifyIcon
        {
            Icon = Program.ApplicationIcon,
            Text = $"{Program.Name} v{Program.Version}",
            Visible = true
        };
        
        var trayMenu = new ContextMenuStrip();

        trayMenu.Items.Add(new ToolStripMenuItem("&Show Settings", null, this.ShowSettingsMenuItemClickEvent));
        trayMenu.Items.Add(new ToolStripSeparator());
        
        var count = _service.GetMonitorCount();

        if (count > 1)
        {
            var monitorsMenuItem = new ToolStripMenuItem("&Change Wallpaper");

            monitorsMenuItem.DropDownItems.Add(new ToolStripMenuItem("All Monitors", null, this.ChangeWallpaperMenuItemClickEvent));
            monitorsMenuItem.DropDownItems.Add(new ToolStripSeparator());

            for (uint i = 0; i < count; i++)
            {
                var id = _service.GetMonitorId(i);
                
                monitorsMenuItem.DropDownItems.Add(
                    new ToolStripMenuItem($"Monitor #{i + 1}", null, this.ChangeWallpaperMenuItemClickEvent)
                    {
                        Tag = id
                    });
            }

            trayMenu.Items.Add(monitorsMenuItem);
        }
        else
        {
            trayMenu.Items.Add(new ToolStripMenuItem("&Change Wallpaper", null, this.ChangeWallpaperMenuItemClickEvent));
        }
        
        trayMenu.Items.Add(new ToolStripSeparator());
        trayMenu.Items.Add(new ToolStripMenuItem("&About...", null, this.AboutMenuItemClickEvent));
        trayMenu.Items.Add(new ToolStripMenuItem("E&xit", null, this.ExitMenuItemClickEvent));
        
        notifyIcon.ContextMenuStrip = trayMenu;
        notifyIcon.MouseDoubleClick += this.ShowSettingsMenuItemClickEvent;
    }

    /// <summary>
    /// Set up the tray form.
    /// </summary>
    private void SetupForm()
    {
        this.ShowInTaskbar = false;
        this.StartPosition = FormStartPosition.Manual;
        this.Text = $"{Program.Name} v{Program.Version}";
        this.Visible = false;
        this.WindowState = FormWindowState.Minimized;

        this.Shown += (_, _) =>
        {
            this.Visible = false;
        };
    }

    /// <summary>
    /// Show the about-program dialog.
    /// </summary>
    private void AboutMenuItemClickEvent(object? _, EventArgs e)
    {
        foreach (Form form in Application.OpenForms)
        {
            if (form is not AboutDialog)
            {
                continue;
            }

            form.Focus();
            return;
        }
        
        using var dialog = new AboutDialog();
        dialog.ShowDialog(this);
    }

    /// <summary>
    /// Change wallpaper for all or specific monitor.
    /// </summary>
    private void ChangeWallpaperMenuItemClickEvent(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem { Tag: not null } menuItem)
        {
            var monitorId = menuItem.Tag.ToString();

            if (monitorId is null)
            {
                MessageBox.Show(
                    "Unable to get monitor ID from menu item.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }
            
            _service.SetWallpaper(monitorId);
        }
        else
        {
            _service.SetWallpapers();
        }
    }

    /// <summary>
    /// Close all windows and exit the application.
    /// </summary>
    private void ExitMenuItemClickEvent(object? _, EventArgs e)
    {
        _service.StopAutoChanger();
        Application.Exit();
    }

    /// <summary>
    /// Show the settings dialog.
    /// </summary>
    private void ShowSettingsMenuItemClickEvent(object? _, EventArgs e)
    {
        foreach (Form form in Application.OpenForms)
        {
            if (form is not SettingsDialog)
            {
                continue;
            }

            form.Focus();
            return;
        }
        
        using var dialog = new SettingsDialog();

        if (dialog.ShowDialog(this) is not DialogResult.OK)
        {
            return;
        }
        
        Config.Load();

        _service.IndexFiles();

        if (Config.Values.AutoChangeInterval is not null)
        {
            _service.StartAutoChanger();
        }
        else
        {
            _service.StopAutoChanger();
        }

        Program.SetProgramAutoStart(Config.Values.AutoStart);
    }
}