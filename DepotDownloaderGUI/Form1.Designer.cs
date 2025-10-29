namespace DepotDownloaderGUI;

partial class Form1
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
        var resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        menuStrip = new MenuStrip();
        fileMenu = new ToolStripMenuItem();
        saveMenuItem = new ToolStripMenuItem();
        exitMenuItem = new ToolStripMenuItem();
        environmentMenu = new ToolStripMenuItem();
        defaultMenuItem = new ToolStripMenuItem();
        localMenuItem = new ToolStripMenuItem();
        addMenuItem = new ToolStripMenuItem();
        labelAppId = new Label();
        textBoxAppId = new TextBox();
        buttonSearch = new Button();
        pictureBoxIcon = new PictureBox();
        labelGameName = new Label();
        labelDescription = new Label();
        buttonDownload = new Button();
        downloadContextMenu = new ContextMenuStrip();
        downloadMenuItem = new ToolStripMenuItem();
        uploadToClientMenuItem = new ToolStripMenuItem();
        buttonGetDepotKeys = new Button();
        progressBar = new ProgressBar();
        logBox = new RichTextBox();
        labelProgress = new Label();
        panelIconPlaceholder = new Panel();
        panelDescriptionPlaceholder = new Panel();
        labelClientStatus = new Label();
        menuStrip.SuspendLayout();
        downloadContextMenu.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).BeginInit();
        SuspendLayout();
        // 
        // menuStrip
        // 
        menuStrip.ImageScalingSize = new Size(20, 20);
        menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, environmentMenu });
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
        // environmentMenu
        // 
        environmentMenu.DropDownItems.AddRange(new ToolStripItem[] { defaultMenuItem, localMenuItem, addMenuItem });
        environmentMenu.Name = "environmentMenu";
        environmentMenu.Size = new Size(106, 24);
        environmentMenu.Text = "Environment";
        // 
        // defaultMenuItem
        // 
        defaultMenuItem.Checked = true;
        defaultMenuItem.CheckState = CheckState.Checked;
        defaultMenuItem.Name = "defaultMenuItem";
        defaultMenuItem.Size = new Size(141, 26);
        defaultMenuItem.Text = "Default";
        // 
        // localMenuItem
        // 
        localMenuItem.Name = "localMenuItem";
        localMenuItem.Size = new Size(141, 26);
        localMenuItem.Text = "Local";
        // 
        // addMenuItem
        // 
        addMenuItem.Name = "addMenuItem";
        addMenuItem.Size = new Size(141, 26);
        addMenuItem.Text = "Add";
        // 
        // labelAppId
        // 
        labelAppId.AutoSize = true;
        labelAppId.Location = new Point(23, 38);
        labelAppId.Name = "labelAppId";
        labelAppId.Size = new Size(59, 20);
        labelAppId.TabIndex = 0;
        labelAppId.Text = "App ID:";
        // 
        // textBoxAppId
        // 
        textBoxAppId.Location = new Point(88, 34);
        textBoxAppId.Margin = new Padding(3, 4, 3, 4);
        textBoxAppId.Name = "textBoxAppId";
        textBoxAppId.Size = new Size(137, 27);
        textBoxAppId.TabIndex = 1;
        // 
        // buttonSearch
        // 
        buttonSearch.Location = new Point(231, 32);
        buttonSearch.Margin = new Padding(3, 4, 3, 4);
        buttonSearch.Name = "buttonSearch";
        buttonSearch.Size = new Size(86, 31);
        buttonSearch.TabIndex = 2;
        buttonSearch.Text = "Search";
        buttonSearch.UseVisualStyleBackColor = true;
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
        labelGameName.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
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
        // buttonDownload
        // 
        buttonDownload.Location = new Point(23, 280);
        buttonDownload.Margin = new Padding(3, 4, 3, 4);
        buttonDownload.Name = "buttonDownload";
        buttonDownload.Size = new Size(137, 40);
        buttonDownload.TabIndex = 6;
        buttonDownload.Text = "Download";
        buttonDownload.UseVisualStyleBackColor = true;
        buttonDownload.Click += ButtonDownload_Click;
        // 
        // downloadContextMenu
        // 
        downloadContextMenu.ImageScalingSize = new Size(20, 20);
        downloadContextMenu.Items.AddRange(new ToolStripItem[] { downloadMenuItem, uploadToClientMenuItem });
        downloadContextMenu.Name = "downloadContextMenu";
        downloadContextMenu.Size = new Size(224, 52);
        // 
        // downloadMenuItem
        // 
        downloadMenuItem.Name = "downloadMenuItem";
        downloadMenuItem.Size = new Size(223, 24);
        downloadMenuItem.Text = "Download";
        // 
        // uploadToClientMenuItem
        // 
        uploadToClientMenuItem.Name = "uploadToClientMenuItem";
        uploadToClientMenuItem.Size = new Size(223, 24);
        uploadToClientMenuItem.Text = "Upload to Client";
        // 
        // buttonGetDepotKeys
        // 
        buttonGetDepotKeys.Location = new Point(201, 280);
        buttonGetDepotKeys.Margin = new Padding(3, 4, 3, 4);
        buttonGetDepotKeys.Name = "buttonGetDepotKeys";
        buttonGetDepotKeys.Size = new Size(137, 40);
        buttonGetDepotKeys.TabIndex = 7;
        buttonGetDepotKeys.Text = "Get depotKeys";
        buttonGetDepotKeys.UseVisualStyleBackColor = true;
        // 
        // progressBar
        // 
        progressBar.Location = new Point(23, 347);
        progressBar.Margin = new Padding(3, 4, 3, 4);
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(857, 31);
        progressBar.TabIndex = 8;
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
        // labelProgress
        // 
        labelProgress.AutoSize = true;
        labelProgress.Location = new Point(23, 387);
        labelProgress.Name = "labelProgress";
        labelProgress.Size = new Size(0, 20);
        labelProgress.TabIndex = 9;
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
        // labelClientStatus
        // 
        labelClientStatus.AutoSize = true;
        labelClientStatus.Location = new Point(23, 700);
        labelClientStatus.Name = "labelClientStatus";
        labelClientStatus.Size = new Size(0, 20);
        labelClientStatus.TabIndex = 13;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(914, 720);
        Controls.Add(menuStrip);
        Controls.Add(labelAppId);
        Controls.Add(textBoxAppId);
        Controls.Add(buttonSearch);
        Controls.Add(pictureBoxIcon);
        Controls.Add(labelGameName);
        Controls.Add(labelDescription);
        Controls.Add(buttonDownload);
        Controls.Add(buttonGetDepotKeys);
        Controls.Add(progressBar);
        Controls.Add(labelProgress);
        Controls.Add(logBox);
        Controls.Add(panelIconPlaceholder);
        Controls.Add(panelDescriptionPlaceholder);
        Controls.Add(labelClientStatus);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Icon = (Icon)resources.GetObject("$this.Icon");
        MainMenuStrip = menuStrip;
        Margin = new Padding(3, 4, 3, 4);
        MaximizeBox = false;
        Name = "Form1";
        Text = "steamDepot - develop";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        downloadContextMenu.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)pictureBoxIcon).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip;
    private System.Windows.Forms.ToolStripMenuItem fileMenu;
    private System.Windows.Forms.ToolStripMenuItem environmentMenu;
    private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
    private System.Windows.Forms.ToolStripMenuItem defaultMenuItem;
    private System.Windows.Forms.ToolStripMenuItem localMenuItem;
    private System.Windows.Forms.ToolStripMenuItem addMenuItem;
    private System.Windows.Forms.Label labelAppId;
    private System.Windows.Forms.TextBox textBoxAppId;
    private System.Windows.Forms.Button buttonSearch;
    private System.Windows.Forms.PictureBox pictureBoxIcon;
    private System.Windows.Forms.Label labelGameName;
    private System.Windows.Forms.Label labelDescription;
    private System.Windows.Forms.Button buttonDownload;
    private System.Windows.Forms.ContextMenuStrip downloadContextMenu;
    private System.Windows.Forms.ToolStripMenuItem downloadMenuItem;
    private System.Windows.Forms.ToolStripMenuItem uploadToClientMenuItem;
    private System.Windows.Forms.Button buttonGetDepotKeys;
    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.RichTextBox logBox;
    private System.Windows.Forms.Label labelProgress;
    private System.Windows.Forms.Panel panelIconPlaceholder;
    private System.Windows.Forms.Panel panelDescriptionPlaceholder;
    private System.Windows.Forms.Label labelClientStatus;
}
