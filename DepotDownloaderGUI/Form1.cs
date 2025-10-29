using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Text.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace DepotDownloaderGUI;

public partial class Form1 : Form
{
    private Process? currentProcess;
    private bool isDownloading = false;
    private string manifestRepoUrl = "https://github.com/SteamAutoCracks/ManifestHub";
    private string manifestRepoPath = "";
    private TcpListener? server;
    private bool isServerRunning = false;
    private const int SERVER_PORT = 5000;
    private TcpClient? connectedClient;
    private bool hasClient = false;

    public Form1()
    {
        InitializeComponent();
        buttonSearch.Click += async (s, e) => await SearchAppIdAsync();
        textBoxAppId.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) { await SearchAppIdAsync(); e.Handled = true; e.SuppressKeyPress = true; } };
        buttonDownload.Click += ButtonDownload_Click;
        downloadMenuItem.Click += DownloadMenuItem_Click;
        uploadToClientMenuItem.Click += UploadToClient_Click;
        buttonGetDepotKeys.Click += ButtonGetDepotKeys_Click;
        this.FormClosing += Form1_FormClosing;

        // Menu event handlers
        saveMenuItem.Click += SaveMenuItem_Click;
        exitMenuItem.Click += ExitMenuItem_Click;
        defaultMenuItem.Click += DefaultMenuItem_Click;
        localMenuItem.Click += LocalMenuItem_Click;
        addMenuItem.Click += AddMenuItem_Click;

        // Start server when form loads
        StartServer();
    }

    private async void StartServer()
    {
        try
        {
            server = new TcpListener(IPAddress.Any, SERVER_PORT);
            server.Start();
            isServerRunning = true;

            // Get local IP address
            string localIP = "";
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint? endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint?.Address.ToString() ?? "unknown";
            }

            AppendLog($"Server started successfully");
            AppendLog($"Listening on all interfaces (0.0.0.0) port {SERVER_PORT}");
            AppendLog($"Local IP address: {localIP}");
            AppendLog($"To connect from another computer, use: {localIP}:{SERVER_PORT}");

            // Start accepting clients in a separate task
            _ = Task.Run(AcceptClientsAsync);
        }
        catch (Exception ex)
        {
            AppendLog($"Failed to start server: {ex.Message}");
            AppendLog($"Stack trace: {ex.StackTrace}");
            isServerRunning = false;
        }
    }

    private async Task AcceptClientsAsync()
    {
        while (isServerRunning && server != null)
        {
            try
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClientAsync(client));
            }
            catch (Exception ex)
            {
                if (isServerRunning)
                {
                    AppendLog($"Error accepting client: {ex.Message}");
                }
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            connectedClient = client;
            hasClient = true;
            UpdateClientStatus();

            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new StreamReader(stream);
            using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            string? request = await reader.ReadLineAsync();
            if (request != null)
            {
                AppendLog($"Received request from client: {request}");
                // TODO: Handle client requests
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error handling client: {ex.Message}");
        }
        finally
        {
            if (connectedClient == client)
            {
                connectedClient = null;
                hasClient = false;
                UpdateClientStatus();
            }
            client.Close();
        }
    }

    private void UpdateClientStatus()
    {
        if (labelClientStatus.InvokeRequired)
        {
            labelClientStatus.Invoke(() => UpdateClientStatus());
            return;
        }
        labelClientStatus.Text = hasClient ? "Client connected" : "No client connected";
        labelClientStatus.ForeColor = hasClient ? Color.Green : Color.Red;
    }

    private async Task SendToClient(string json)
    {
        if (connectedClient == null || !hasClient)
        {
            MessageBox.Show("No client connected.", "Error");
            return;
        }

        try
        {
            using NetworkStream stream = connectedClient.GetStream();
            using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
            
            // Send the length of the JSON first
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] lengthBytes = BitConverter.GetBytes(jsonBytes.Length);
            
            AppendLog($"Sending JSON length: {jsonBytes.Length} bytes");
            AppendLog($"JSON content: {json}");
            
            await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
            await stream.WriteAsync(jsonBytes, 0, jsonBytes.Length);
            await stream.FlushAsync();
            
            AppendLog("Successfully sent download information to client");
        }
        catch (Exception ex)
        {
            AppendLog($"Error sending to client: {ex.Message}");
            AppendLog($"Stack trace: {ex.StackTrace}");
            MessageBox.Show("Failed to send to client: " + ex.Message, "Error");
        }
    }

    private void StopServer()
    {
        isServerRunning = false;
        server?.Stop();
        AppendLog("Server stopped");
    }

    private void SaveMenuItem_Click(object? sender, EventArgs e)
    {
        if (isDownloading)
        {
            MessageBox.Show("Cannot save while a download is in progress.", "Warning");
            return;
        }

        var settings = new
        {
            ManifestRepoUrl = manifestRepoUrl,
            ManifestRepoPath = manifestRepoPath
        };

        try
        {
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllText("settings.json", json);
            MessageBox.Show("Settings saved successfully.", "Success");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error");
        }
    }

    private void ExitMenuItem_Click(object? sender, EventArgs e)
    {
        if (isDownloading)
        {
            var result = MessageBox.Show("A download is in progress. Are you sure you want to exit?", 
                "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
                return;
            currentProcess?.Kill();
        }
        Application.Exit();
    }

    private void DefaultMenuItem_Click(object? sender, EventArgs e)
    {
        manifestRepoUrl = "https://github.com/SteamAutoCracks/ManifestHub";
        manifestRepoPath = "";
        defaultMenuItem.Checked = true;
        localMenuItem.Checked = false;
        addMenuItem.Checked = false;
    }

    private void LocalMenuItem_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Select local manifest repository folder",
            UseDescriptionForTitle = true
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            manifestRepoPath = dialog.SelectedPath;
            manifestRepoUrl = "";
            defaultMenuItem.Checked = false;
            localMenuItem.Checked = true;
            addMenuItem.Checked = false;
        }
    }

    private void AddMenuItem_Click(object? sender, EventArgs e)
    {
        using var dialog = new Form
        {
            Text = "Add Repository",
            Size = new Size(400, 150),
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var label = new Label
        {
            Text = "Enter repository URL:",
            Location = new Point(10, 20),
            AutoSize = true
        };

        var textBox = new TextBox
        {
            Location = new Point(10, 40),
            Width = 360
        };

        var buttonOk = new Button
        {
            Text = "OK",
            DialogResult = DialogResult.OK,
            Location = new Point(200, 70)
        };

        var buttonCancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Location = new Point(290, 70)
        };

        dialog.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
        dialog.AcceptButton = buttonOk;
        dialog.CancelButton = buttonCancel;

        if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(textBox.Text))
        {
            manifestRepoUrl = textBox.Text.Trim();
            manifestRepoPath = "";
            defaultMenuItem.Checked = false;
            localMenuItem.Checked = false;
            addMenuItem.Checked = true;
        }
    }

    private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (isDownloading)
        {
            var result = MessageBox.Show("A download is in progress. Are you sure you want to cancel and exit?", 
                "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            currentProcess?.Kill();
        }
        StopServer();
    }

    private void ShowProgressControls(bool show)
    {
        logBox.Clear();
        progressBar.Value = 0;
        labelProgress.Text = "Ready";
    }

    private void AppendLog(string message)
    {
        if (logBox.InvokeRequired)
        {
            logBox.Invoke(() => AppendLog(message));
            return;
        }
        logBox.AppendText(message + Environment.NewLine);
        logBox.ScrollToCaret();
    }

    private void UpdateProgress(int value, string status)
    {
        if (progressBar.InvokeRequired)
        {
            progressBar.Invoke(() => UpdateProgress(value, status));
            return;
        }
        progressBar.Value = value;
        labelProgress.Text = status;
    }

    private async Task SearchAppIdAsync()
    {
        string appId = textBoxAppId.Text.Trim();
        if (string.IsNullOrEmpty(appId)) return;
        string url = $"https://store.steampowered.com/api/appdetails?appids={appId}";
        try
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            var appData = root.GetProperty(appId);
            if (appData.GetProperty("success").GetBoolean())
            {
                var data = appData.GetProperty("data");
                labelGameName.Text = data.GetProperty("name").GetString() ?? "";
                labelDescription.Text = data.GetProperty("detailed_description").GetString() ?? "";
                string iconUrl = data.GetProperty("header_image").GetString() ?? "";
                if (!string.IsNullOrEmpty(iconUrl))
                {
                    var imgStream = await client.GetStreamAsync(iconUrl);
                    pictureBoxIcon.Image = Image.FromStream(imgStream);
                    pictureBoxIcon.Visible = true;
                    panelIconPlaceholder.Visible = false;
                }
                else
                {
                    pictureBoxIcon.Image = null;
                    pictureBoxIcon.Visible = false;
                    panelIconPlaceholder.Visible = true;
                }
                labelDescription.Visible = true;
                panelDescriptionPlaceholder.Visible = false;
            }
            else
            {
                labelGameName.Text = "Game not found.";
                labelDescription.Text = "";
                labelDescription.Visible = false;
                panelDescriptionPlaceholder.Visible = true;
                pictureBoxIcon.Image = null;
                pictureBoxIcon.Visible = false;
                panelIconPlaceholder.Visible = true;
            }
        }
        catch
        {
            labelGameName.Text = "Error fetching game info.";
            labelDescription.Text = "";
            labelDescription.Visible = false;
            panelDescriptionPlaceholder.Visible = true;
            pictureBoxIcon.Image = null;
            pictureBoxIcon.Visible = false;
            panelIconPlaceholder.Visible = true;
        }
    }

    private async Task<bool> GetDepotKeyAsync(string appId)
    {
        try
        {
            string keyContent;
            if (!string.IsNullOrEmpty(manifestRepoPath))
            {
                // Local repository
                string keyPath = Path.Combine(manifestRepoPath, appId, "key.vdf");
                if (!File.Exists(keyPath))
                {
                    AppendLog($"Key file not found at: {keyPath}");
                    return false;
                }
                keyContent = await File.ReadAllTextAsync(keyPath);
            }
            else
            {
                // Remote repository
                string url = $"{manifestRepoUrl}/raw/{appId}/key.vdf";
                using var client = new HttpClient();
                keyContent = await client.GetStringAsync(url);
            }
            
            // Create manifests directory if it doesn't exist
            Directory.CreateDirectory("manifests");

            // Convert VDF format to required format
            var lines = new List<string>();
            var matches = Regex.Matches(keyContent, @"""(\d+)""\s*{\s*""DecryptionKey""\s+""([^""]+)""");
            foreach (Match match in matches)
            {
                string depotId = match.Groups[1].Value;
                string key = match.Groups[2].Value;
                lines.Add($"{depotId};{key}");
            }

            // Save as appid.key
            string keyFilePath = Path.Combine("manifests", $"{appId}.key");
            await File.WriteAllLinesAsync(keyFilePath, lines);
            AppendLog($"Saved depot keys to {keyFilePath}");
            return true;
        }
        catch (Exception ex)
        {
            AppendLog($"Error downloading depot keys: {ex.Message}");
            return false;
        }
    }

    private async Task DownloadManifestFilesAsync(string appId)
    {
        try
        {
            if (!string.IsNullOrEmpty(manifestRepoPath))
            {
                // Local repository
                string manifestDir = Path.Combine(manifestRepoPath, appId);
                if (!Directory.Exists(manifestDir))
                {
                    AppendLog($"Manifest directory not found at: {manifestDir}");
                    return;
                }

                Directory.CreateDirectory("manifests");
                foreach (string manifestFile in Directory.GetFiles(manifestDir, "*.manifest"))
                {
                    string fileName = Path.GetFileName(manifestFile);
                    string destPath = Path.Combine("manifests", fileName);
                    File.Copy(manifestFile, destPath, true);
                    AppendLog($"Copied manifest: {fileName}");
                }
            }
            else
            {
                // Remote repository - use app ID as branch name
                string url = $"{manifestRepoUrl}/tree/{appId}";
                using var client = new HttpClient();
                var response = await client.GetStringAsync(url);
                
                // Extract manifest file URLs
                var matches = Regex.Matches(response, @"href=""([^""]+\.manifest)""");
                foreach (Match match in matches)
                {
                    // Construct the correct raw URL with refs/heads
                    string manifestUrl = $"{manifestRepoUrl}/raw/refs/heads/{appId}/{Path.GetFileName(match.Groups[1].Value)}";
                    string fileName = Path.GetFileName(manifestUrl);
                    string localPath = Path.Combine("manifests", fileName);
                    
                    Directory.CreateDirectory("manifests");
                    
                    try
                    {
                        // Download as bytes to preserve exact content
                        var manifestBytes = await client.GetByteArrayAsync(manifestUrl);
                        await File.WriteAllBytesAsync(localPath, manifestBytes);
                        AppendLog($"Downloaded manifest: {fileName} ({manifestBytes.Length} bytes)");
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"Failed to download manifest {fileName}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error downloading manifest files: {ex.Message}");
        }
    }

    private void ButtonGetDepotKeys_Click(object? sender, EventArgs e)
    {
        if (isDownloading)
        {
            MessageBox.Show("A download is in progress. Please wait.", "Warning");
            return;
        }

        string appId = textBoxAppId.Text.Trim();
        if (string.IsNullOrEmpty(appId))
        {
            MessageBox.Show("Please enter an App ID first.", "Warning");
            return;
        }

        ShowProgressControls(true);
        AppendLog("Fetching depot keys and manifest files...");

        Task.Run(async () =>
        {
            try
            {
                bool keySuccess = await GetDepotKeyAsync(appId);
                if (keySuccess)
                {
                    await DownloadManifestFilesAsync(appId);
                }
            }
            catch (Exception ex)
            {
                AppendLog($"Error: {ex.Message}");
            }
        });
    }

    private void ButtonDownload_Click(object? sender, EventArgs e)
    {
        if (buttonDownload != null)
        {
            downloadContextMenu.Show(buttonDownload, new Point(0, buttonDownload.Height));
        }
    }

    private void DownloadMenuItem_Click(object? sender, EventArgs e)
    {
        if (isDownloading)
        {
            MessageBox.Show("A download is already in progress.", "Warning");
            return;
        }

        using var dialog = new DownloadDialog(textBoxAppId.Text.Trim());
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                // Get depot key and manifest files
                string depotKeysFile = Path.Combine("manifests", $"{dialog.AppId}.key");
                string manifestFile = Path.Combine("manifests", $"{dialog.DepotId}_*.manifest");
                string[] manifestFiles = Directory.GetFiles("manifests", $"{dialog.DepotId}_*.manifest");
                
                if (manifestFiles.Length == 0)
                {
                    MessageBox.Show("No manifest file found for the selected depot.", "Error");
                    return;
                }

                string manifestId = Path.GetFileNameWithoutExtension(manifestFiles[0]).Split('_')[1];
                string args = $"-app {dialog.AppId} -depotkeys \"{depotKeysFile}\" -depot {dialog.DepotId} -manifest {manifestId} -manifestfile \"{manifestFiles[0]}\"";
                
                isDownloading = true;
                ShowProgressControls(true);
                AppendLog("Starting download...");
                AppendLog($"Arguments: {args}");

                var psi = new ProcessStartInfo
                {
                    FileName = "DepotDownloaderMod.exe",
                    Arguments = args,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                currentProcess = new Process { StartInfo = psi };
                currentProcess.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        AppendLog(e.Data);
                        // Try to parse progress from output
                        var match = Regex.Match(e.Data, @"(\d+)%");
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int progress))
                        {
                            UpdateProgress(progress, e.Data);
                        }
                    }
                };
                currentProcess.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        AppendLog($"ERROR: {e.Data}");
                    }
                };

                currentProcess.Start();
                currentProcess.BeginOutputReadLine();
                currentProcess.BeginErrorReadLine();
                currentProcess.EnableRaisingEvents = true;
                currentProcess.Exited += (s, e) =>
                {
                    isDownloading = false;
                    currentProcess = null;
                    if (this.InvokeRequired)
                    {
                        this.Invoke(() =>
                        {
                            UpdateProgress(100, "Download completed");
                            AppendLog("Download process completed.");
                        });
                    }
                };
            }
            catch (Exception ex)
            {
                isDownloading = false;
                currentProcess = null;
                MessageBox.Show(this, "Failed to start download: " + ex.Message, "Error");
                ShowProgressControls(false);
            }
        }
    }

    private async void UploadToClient_Click(object? sender, EventArgs e)
    {
        if (isDownloading)
        {
            MessageBox.Show("A download is already in progress.", "Warning");
            return;
        }

        if (!hasClient)
        {
            MessageBox.Show("No client connected.", "Error");
            return;
        }

        using var dialog = new DownloadDialog(textBoxAppId.Text.Trim());
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                // Create a JSON object with only AppId and DepotId
                var downloadInfo = new
                {
                    AppId = dialog.AppId,
                    DepotId = dialog.DepotId
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(downloadInfo, options);
                _ = SendToClient(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Failed to prepare upload: " + ex.Message, "Error");
            }
        }
    }
}
