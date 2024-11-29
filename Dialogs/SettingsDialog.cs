namespace Mural.Dialogs;

public class SettingsDialog : Form
{
    #region Controls

    /// <summary>
    /// Wallpapers path textbox.
    /// </summary>
    private readonly TextBox _wallpaperPathTextBox = new()
    {
        Location = new(15, 35),
        Width = 400
    };

    /// <summary>
    /// Browse wallpapers path button.
    /// </summary>
    private readonly Button _browsePathButton = new()
    {
        Location = new(415, 33),
        Size = new(50, 28),
        Text = "..."
    };

    /// <summary>
    /// Checkbox for getting files recursively.
    /// </summary>
    private readonly CheckBox _getFilesRecursivelyCheckBox = new()
    {
        AutoSize = true,
        Location = new(18, 65),
        Text = "Get files recursively"
    };

    /// <summary>
    /// Checkbox for shuffling files.
    /// </summary>
    private readonly CheckBox _shuffleImagesCheckBox = new()
    {
        AutoSize = true,
        Location = new(18, 86),
        Text = "Shuffle images"
    };

    /// <summary>
    /// Checkbox for toggling auto-changing of wallpapers.
    /// </summary>
    private readonly CheckBox _autoChangeCheckBox = new()
    {
        AutoSize = true,
        Location = new(18, 125),
        Text = "Automatically change wallpapers on a set interval"
    };

    /// <summary>
    /// Interval of auto-change.
    /// </summary>
    private readonly NumericUpDown _autoChangeIntervalNumeric = new()
    {
        Increment = 1,
        Location = new(50, 145),
        Maximum = 9999,
        Minimum = 1,
        Width = 100
    };

    /// <summary>
    /// Checkbox for toggling auto-start on Windows login.
    /// </summary>
    private readonly CheckBox _autoStartCheckBox = new()
    {
        AutoSize = true,
        Location = new(18, 185),
        Text = $"Automatically start {Program.Name} when logging into Windows"
    };

    /// <summary>
    /// Dialog ok button.
    /// </summary>
    private readonly Button _okButton = new()
    {
        DialogResult = DialogResult.OK,
        Location = new(415, 250),
        Size = new(100, 40),
        Text = "&Ok"
    };

    /// <summary>
    /// Dialog cancel button.
    /// </summary>
    private readonly Button _cancelButton = new()
    {
        DialogResult = DialogResult.Cancel,
        Location = new(300, 250),
        Size = new(100, 40),
        Text = "&Cancel"
    };
    
    #endregion
    
    #region Constructor functions

    /// <summary>
    /// Set up the settings dialog.
    /// </summary>
    public SettingsDialog()
    {
        this.SetupControls();
        this.SetupForm();
    }

    /// <summary>
    /// Add controls and events.
    /// </summary>
    private void SetupControls()
    {
        _wallpaperPathTextBox.Text = Config.Values.WallpaperPath;
        _getFilesRecursivelyCheckBox.Checked = Config.Values.GetFilesRecursively;
        _shuffleImagesCheckBox.Checked = Config.Values.ShuffleImages;
        _autoChangeCheckBox.Checked = Config.Values.AutoChangeInterval is not null;
        _autoChangeIntervalNumeric.Enabled = Config.Values.AutoChangeInterval is not null;
        _autoChangeIntervalNumeric.Value = (decimal)(Config.Values.AutoChangeInterval ?? 1);
        _autoStartCheckBox.Checked = Config.Values.AutoStart && Program.IsSetToAutoStart();

        _browsePathButton.Click += this.BrowsePathButtonClickEvent;
        _cancelButton.Click += (_, _) => this.Close();
        _okButton.Click += this.OkButtonClickEvent;

        _autoChangeCheckBox.CheckedChanged +=
            (_, _) => _autoChangeIntervalNumeric.Enabled = _autoChangeCheckBox.Checked;

        this.Controls.Add(this.CreateLabel("Set wallpapers path:", new(15, 15)));
        this.Controls.Add(_wallpaperPathTextBox);
        this.Controls.Add(_browsePathButton);
        this.Controls.Add(_getFilesRecursivelyCheckBox);
        this.Controls.Add(_shuffleImagesCheckBox);
        
        this.Controls.Add(_autoChangeCheckBox);
        this.Controls.Add(this.CreateLabel("Every", new(15, 148)));
        this.Controls.Add(_autoChangeIntervalNumeric);
        this.Controls.Add(this.CreateLabel("minutes", new(151, 148)));
        
        this.Controls.Add(_autoStartCheckBox);

        this.Controls.Add(_cancelButton);
        this.Controls.Add(_okButton);

        this.AcceptButton = _okButton;
        this.CancelButton = _cancelButton;
    }

    /// <summary>
    /// Set up the settings dialog.
    /// </summary>
    private void SetupForm()
    {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.Icon = Program.ApplicationIcon;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Size = new(550, 350);
        this.Text = $"{Program.Name} Settings";
        this.StartPosition = FormStartPosition.CenterScreen;
    }
    
    #endregion
    
    #region Controls event functions

    /// <summary>
    /// Browse for a new wallpapers' path.
    /// </summary>
    private void BrowsePathButtonClickEvent(object? _, EventArgs e)
    {
        var dialog = new FolderBrowserDialog
        {
            Description = "Select the folder you wish to get wallpapers from",
            SelectedPath = _wallpaperPathTextBox.Text,
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog(this) is not DialogResult.OK)
        {
            return;
        }

        _wallpaperPathTextBox.Text = dialog.SelectedPath;
    }

    /// <summary>
    /// Save config and close dialog.
    /// </summary>
    private void OkButtonClickEvent(object? _, EventArgs e)
    {
        Config.Values.AutoChangeInterval = _autoChangeCheckBox.Checked
            ? (double)_autoChangeIntervalNumeric.Value
            : null;

        Config.Values.AutoStart = _autoStartCheckBox.Checked;
        Config.Values.GetFilesRecursively = _getFilesRecursivelyCheckBox.Checked;
        Config.Values.ShuffleImages = _shuffleImagesCheckBox.Checked;
        Config.Values.WallpaperPath = _wallpaperPathTextBox.Text.Trim();
        Config.Save();
        
        this.Close();
    }
    
    #endregion
    
    #region Helper functions

    /// <summary>
    /// Create a new label.
    /// </summary>
    /// <param name="text">Label text.</param>
    /// <param name="location">Label location.</param>
    /// <returns>Label.</returns>
    private Label CreateLabel(string text, Point location)
    {
        return new()
        {
            AutoSize = true,
            Location = location,
            Text = text
        };
    }
    
    #endregion
}