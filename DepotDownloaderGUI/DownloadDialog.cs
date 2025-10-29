using System.Windows.Forms;

namespace DepotDownloaderGUI
{
    public partial class DownloadDialog : Form
    {
        public string AppId { get; private set; }
        public string DepotId { get; private set; }

        public DownloadDialog(string appId)
        {
            InitializeComponent();
            textBoxAppId.Text = appId;
            textBoxAppId.ReadOnly = true;
        }

        private void InitializeComponent()
        {
            this.Text = "Download Settings";
            this.Size = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var labelAppId = new Label
            {
                Text = "App ID:",
                Location = new Point(10, 20),
                AutoSize = true
            };

            textBoxAppId = new TextBox
            {
                Location = new Point(120, 17),
                Width = 250
            };

            var labelDepotId = new Label
            {
                Text = "Depot ID:",
                Location = new Point(10, 50),
                AutoSize = true
            };

            textBoxDepotId = new TextBox
            {
                Location = new Point(120, 47),
                Width = 250
            };

            var buttonOk = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(200, 120)
            };

            var buttonCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(290, 120)
            };

            this.Controls.AddRange(new Control[] { labelAppId, textBoxAppId, labelDepotId, textBoxDepotId, buttonOk, buttonCancel });
            this.AcceptButton = buttonOk;
            this.CancelButton = buttonCancel;

            buttonOk.Click += ButtonOk_Click;
        }

        private void ButtonOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxAppId.Text) || string.IsNullOrWhiteSpace(textBoxDepotId.Text))
            {
                MessageBox.Show("Please enter both App ID and Depot ID.", "Error");
                DialogResult = DialogResult.None;
                return;
            }

            AppId = textBoxAppId.Text.Trim();
            DepotId = textBoxDepotId.Text.Trim();
        }

        private TextBox textBoxAppId;
        private TextBox textBoxDepotId;
    }
} 