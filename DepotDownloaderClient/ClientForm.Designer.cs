namespace DepotDownloaderClient;

partial class ClientForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        menuStrip = new MenuStrip();
        fileMenu = new ToolStripMenuItem();
        saveMenuItem = new ToolStripMenuItem();
        exitMenuItem = new ToolStripMenuItem();
        labelServer = new Label();
        textBoxServer = new TextBox();
        buttonConnect = new Button();
        pictureBoxIcon = new PictureBox();
        labelGameName = new Label();
        labelDescription = new Label();
        panelIconPlaceholder = new Panel();
        panelDescriptionPlaceholder = new Panel();
        progressBar = new ProgressBar();
        labelProgress = new Label();
        logBox = new RichTextBox();
        menuStrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).BeginInit();
        SuspendLayout();
        // 
        // menuStrip
        // 
        menuStrip.ImageScalingSize = new Size(20, 20);
        menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Padding = new Padding(7, 3, 0, 3);
        menuStrip.Size = new Size(914, 30);
        menuStrip.TabIndex = 13;
        menuStrip.Text = "menuStrip1";
        // 
        // fileMenu
        // 
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { saveMenuItem, exitMenuItem });
        fileMenu.Name = "fileMenu";
        fileMenu.Size = new Size(46, 24);
        fileMenu.Text = "File";
        // 
        // saveMenuItem
        // 
        saveMenuItem.Name = "saveMenuItem";
        saveMenuItem.Size = new Size(123, 26);
        saveMenuItem.Text = "Save";
        // 
        // exitMenuItem
        // 
        exitMenuItem.Name = "exitMenuItem";
        exitMenuItem.Size = new Size(123, 26);
        exitMenuItem.Text = "Exit";
        // 
        // labelServer
        // 
        labelServer.AutoSize = true;
        labelServer.Location = new Point(12, 37);
        labelServer.Name = "labelServer";
        labelServer.Size = new Size(108, 20);
        labelServer.TabIndex = 0;
        labelServer.Text = "Server (IP:Port):";
        // 
        // textBoxServer
        // 
        textBoxServer.Location = new Point(126, 34);
        textBoxServer.Margin = new Padding(3, 4, 3, 4);
        textBoxServer.Name = "textBoxServer";
        textBoxServer.Size = new Size(122, 27);
        textBoxServer.TabIndex = 1;
        textBoxServer.Text = "127.0.0.1:5000";
        // 
        // buttonConnect
        // 
        buttonConnect.Location = new Point(254, 32);
        buttonConnect.Margin = new Padding(3, 4, 3, 4);
        buttonConnect.Name = "buttonConnect";
        buttonConnect.Size = new Size(86, 31);
        buttonConnect.TabIndex = 2;
        buttonConnect.Text = "Connect";
        buttonConnect.UseVisualStyleBackColor = true;
        // 
        // pictureBoxIcon
        // 
        pictureBoxIcon.Location = new Point(23, 80);
        pictureBoxIcon.Margin = new Padding(3, 4, 3, 4);
        pictureBoxIcon.Name = "pictureBoxIcon";
        pictureBoxIcon.Size = new Size(315, 172);
        pictureBoxIcon.SizeMode = PictureBoxSizeMode.StretchImage;
        pictureBoxIcon.TabIndex = 3;
        pictureBoxIcon.TabStop = false;
        pictureBoxIcon.Visible = false;
        // 
        // labelGameName
        // 
        labelGameName.AutoSize = true;
        labelGameName.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        labelGameName.Location = new Point(366, 80);
        labelGameName.Name = "labelGameName";
        labelGameName.Size = new Size(0, 28);
        labelGameName.TabIndex = 4;
        // 
        // labelDescription
        // 
        labelDescription.Location = new Point(366, 120);
        labelDescription.Name = "labelDescription";
        labelDescription.Size = new Size(514, 132);
        labelDescription.TabIndex = 5;
        labelDescription.Visible = false;
        // 
        // panelIconPlaceholder
        // 
        panelIconPlaceholder.BorderStyle = BorderStyle.FixedSingle;
        panelIconPlaceholder.Location = new Point(23, 80);
        panelIconPlaceholder.Margin = new Padding(3, 4, 3, 4);
        panelIconPlaceholder.Name = "panelIconPlaceholder";
        panelIconPlaceholder.Size = new Size(315, 171);
        panelIconPlaceholder.TabIndex = 11;
        // 
        // panelDescriptionPlaceholder
        // 
        panelDescriptionPlaceholder.BorderStyle = BorderStyle.FixedSingle;
        panelDescriptionPlaceholder.Location = new Point(366, 120);
        panelDescriptionPlaceholder.Margin = new Padding(3, 4, 3, 4);
        panelDescriptionPlaceholder.Name = "panelDescriptionPlaceholder";
        panelDescriptionPlaceholder.Size = new Size(514, 131);
        panelDescriptionPlaceholder.TabIndex = 12;
        // 
        // progressBar
        // 
        progressBar.Location = new Point(23, 347);
        progressBar.Margin = new Padding(3, 4, 3, 4);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(857, 31);
        progressBar.TabIndex = 8;
        // 
        // labelProgress
        // 
        labelProgress.AutoSize = true;
        labelProgress.Location = new Point(23, 387);
        labelProgress.Name = "labelProgress";
        labelProgress.Size = new Size(0, 20);
        labelProgress.TabIndex = 9;
        // 
        // logBox
        // 
        logBox.Location = new Point(23, 427);
        logBox.Margin = new Padding(3, 4, 3, 4);
        logBox.Name = "logBox";
        logBox.ReadOnly = true;
        logBox.Size = new Size(857, 265);
        logBox.TabIndex = 10;
        logBox.Text = "";
        // 
        // ClientForm
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(914, 720);
        Controls.Add(menuStrip);
        Controls.Add(labelServer);
        Controls.Add(textBoxServer);
        Controls.Add(buttonConnect);
        Controls.Add(pictureBoxIcon);
        Controls.Add(labelGameName);
        Controls.Add(labelDescription);
        Controls.Add(progressBar);
        Controls.Add(labelProgress);
        Controls.Add(logBox);
        Controls.Add(panelIconPlaceholder);
        Controls.Add(panelDescriptionPlaceholder);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MainMenuStrip = menuStrip;
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        Name = "ClientForm";
        Text = "steamDepot Client";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileMenu;
    private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
    private System.Windows.Forms.Label labelServer;
    private System.Windows.Forms.TextBox textBoxServer;
    private System.Windows.Forms.Button buttonConnect;
    private System.Windows.Forms.PictureBox pictureBoxIcon;
    private System.Windows.Forms.Label labelGameName;
    private System.Windows.Forms.Label labelDescription;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.Label labelProgress;
    private System.Windows.Forms.RichTextBox logBox;
    private System.Windows.Forms.Panel panelIconPlaceholder;
    private System.Windows.Forms.Panel panelDescriptionPlaceholder;
} 