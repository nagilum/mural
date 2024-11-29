using System.Diagnostics;

namespace Mural.Dialogs;

public class AboutDialog : Form
{
    /// <summary>
    /// URL to GitHub repository.
    /// </summary>
    private const string GitHubRepo = "https://github.com/nagilum/mural";
    
    /// <summary>
    /// Set up the about-program dialog.
    /// </summary>
    public AboutDialog()
    {
        this.SetupForm();
        this.SetupControls();
    }

    /// <summary>
    /// Add controls and events.
    /// </summary>
    private void SetupControls()
    {
        var icon = new PictureBox
        {
            BackgroundImage = Program.ApplicationIcon!.ToBitmap(),
            BackgroundImageLayout = ImageLayout.Center,
            Location = new(0, 24),
            Size = new(this.Width, 64)
        };

        var title = new Label
        {
            Font = new(this.Font, FontStyle.Bold),
            Location = new(0, 100),
            Size = new(this.Width, 20),
            Text = Program.Name,
            TextAlign = ContentAlignment.MiddleCenter
        };

        var version = new Label
        {
            Location = new(0, 120),
            Size = new(this.Width, 20),
            Text = $"Version {Program.Version}",
            TextAlign = ContentAlignment.MiddleCenter
        };

        var copyright = new Label
        {
            Location = new(0, 140),
            Size = new(this.Width, 20),
            Text = "Copyright \u00a9 2024 Stian Hanger",
            TextAlign = ContentAlignment.MiddleCenter
        };

        var link = new LinkLabel
        {
            Location = new(0, 160),
            Size = new(this.Width, 20),
            Text = GitHubRepo,
            TextAlign = ContentAlignment.MiddleCenter
        };

        link.LinkClicked += (_, _) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = GitHubRepo,
                    UseShellExecute = true
                });
            }
            catch
            {
                Clipboard.SetText(GitHubRepo);
            
                MessageBox.Show(
                    $"Unable to open URL in default browser. The URL {GitHubRepo} has been copied to your clipboard.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        };

        var button = new Button
        {
            DialogResult = DialogResult.OK,
            Location = new((this.Width - 100) / 2, 200),
            Size = new(100, 40),
            Text = "&Ok",
            TextAlign = ContentAlignment.MiddleCenter
        };

        button.Click += (_, _) =>
        {
            this.Close();
        };

        this.Controls.Add(icon);
        this.Controls.Add(title);
        this.Controls.Add(version);
        this.Controls.Add(copyright);
        this.Controls.Add(link);
        this.Controls.Add(button);

        this.AcceptButton = button;
        this.CancelButton = button;
    }
    
    /// <summary>
    /// Set up the about-program window.
    /// </summary>
    private void SetupForm()
    {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.Icon = Program.ApplicationIcon;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Size = new(400, 300);
        this.Text = $"About {Program.Name}";
        this.StartPosition = FormStartPosition.CenterScreen;
    }
}