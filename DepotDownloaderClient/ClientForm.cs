using System.Net.Sockets;
using System.Text.Json;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;

namespace DepotDownloaderClient;

public partial class ClientForm : Form
{
    private TcpClient? client;
    private bool isConnected = false;
    private bool isDownloading = false;
    private Process? currentProcess;
    private string manifestRepoUrl = "https://github.com/SteamAutoCracks/ManifestHub";
    private string manifestRepoPath = "";

    public ClientForm()
    {
        InitializeComponent();
        buttonConnect.Click += ButtonConnect_Click;
        this.FormClosing += ClientForm_FormClosing;
    }

    private async void ButtonConnect_Click(object? sender, EventArgs e)
    {
        if (isConnected)
        {
            DisconnectFromServer();
            return;
        }

        string[] parts = textBoxServer.Text.Split(':');
        if (parts.Length != 2 || !int.TryParse(parts[1], out int port))
        {
            MessageBox.Show("Invalid server address format. Use IP:PORT", "Error");
            return;
        }

        string serverIP = parts[0];
        AppendLog($"Attempting to connect to {serverIP}:{port}...");

        try
        {
            client = new TcpClient();
            client.ReceiveTimeout = 5000;  // 5 seconds
            client.SendTimeout = 5000;     // 5 seconds

            AppendLog("Connecting...");
            await client.ConnectAsync(serverIP, port);
            
            if (client.Connected)
            {
                isConnected = true;
                buttonConnect.Text = "Disconnect";
                AppendLog("Successfully connected to server");
                AppendLog($"Local endpoint: {client.Client.LocalEndPoint}");
                AppendLog($"Remote endpoint: {client.Client.RemoteEndPoint}");

                // Start listening for server messages
                _ = Task.Run(ReceiveServerMessagesAsync);
            }
            else
            {
                AppendLog("Connection failed - client not connected after ConnectAsync");
                DisconnectFromServer();
            }
        }
        catch (SocketException ex)
        {
            AppendLog($"Socket error: {ex.Message}");
            AppendLog($"Error code: {ex.ErrorCode}");
            AppendLog($"Socket error code: {ex.SocketErrorCode}");
            MessageBox.Show($"Failed to connect: {ex.Message}", "Error");
            DisconnectFromServer();
        }
        catch (Exception ex)
        {
            AppendLog($"Connection error: {ex.Message}");
            AppendLog($"Stack trace: {ex.StackTrace}");
            MessageBox.Show($"Failed to connect: {ex.Message}", "Error");
            DisconnectFromServer();
        }
    }

    private void DisconnectFromServer()
    {
        client?.Close();
        client = null;
        isConnected = false;
        buttonConnect.Text = "Connect";
        AppendLog("Disconnected from server");
        ClearGameInfo();
    }

    private async Task ReceiveServerMessagesAsync()
    {
        if (client == null) return;

        try
        {
            using NetworkStream stream = client.GetStream();
            byte[] lengthBuffer = new byte[4];

            AppendLog("Waiting for message length...");
            // Read the length of the incoming message
            int bytesRead = await stream.ReadAsync(lengthBuffer, 0, 4);
            if (bytesRead == 0)
            {
                AppendLog("Connection closed by server (0 bytes read)");
                return;
            }

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            AppendLog($"Received message length: {messageLength} bytes");

            if (messageLength <= 0 || messageLength > 1024 * 1024) // Sanity check: max 1MB
            {
                AppendLog($"Invalid message length: {messageLength}");
                return;
            }

            byte[] messageBuffer = new byte[messageLength];
            AppendLog("Reading message content...");

            // Read the actual message
            bytesRead = await stream.ReadAsync(messageBuffer, 0, messageLength);
            if (bytesRead == 0)
            {
                AppendLog("Connection closed by server while reading message");
                return;
            }

            AppendLog($"Read {bytesRead} bytes of message content");
            string message = Encoding.UTF8.GetString(messageBuffer);
            AppendLog($"Received message: {message}");

            try
            {
                var downloadInfo = JsonSerializer.Deserialize<DownloadInfo>(message);
                if (downloadInfo != null)
                {
                    AppendLog("Successfully deserialized download info");
                    AppendLog($"AppId: {downloadInfo.AppId}");
                    AppendLog($"DepotId: {downloadInfo.DepotId}");
                    
                    await FetchGameInfoAsync(downloadInfo);
                    AppendLog("Successfully processed download info");
                }
                else
                {
                    AppendLog("Failed to deserialize download info (null result)");
                }
            }
            catch (JsonException ex)
            {
                AppendLog($"JSON deserialization error: {ex.Message}");
                AppendLog($"Error position: {ex.BytePositionInLine}");
                AppendLog($"Error line number: {ex.LineNumber}");
            }

            // Keep connection open and wait for more messages
            AppendLog("Waiting for more messages...");
            while (client?.Connected == true)
            {
                try
                {
                    // Check if there's data available
                    if (stream.DataAvailable)
                    {
                        bytesRead = await stream.ReadAsync(lengthBuffer, 0, 4);
                        if (bytesRead == 0)
                        {
                            AppendLog("Connection closed by server");
                            break;
                        }

                        messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                        if (messageLength <= 0 || messageLength > 1024 * 1024)
                        {
                            AppendLog($"Invalid message length: {messageLength}");
                            continue;
                        }

                        messageBuffer = new byte[messageLength];
                        bytesRead = await stream.ReadAsync(messageBuffer, 0, messageLength);
                        if (bytesRead == 0)
                        {
                            AppendLog("Connection closed by server while reading message");
                            break;
                        }

                        message = Encoding.UTF8.GetString(messageBuffer);
                        AppendLog($"Received new message: {message}");

                        // Process the new message
                        var newDownloadInfo = JsonSerializer.Deserialize<DownloadInfo>(message);
                        if (newDownloadInfo != null)
                        {
                            await FetchGameInfoAsync(newDownloadInfo);
                        }
                    }
                    else
                    {
                        // Sleep briefly to prevent CPU spinning
                        await Task.Delay(100);
                    }
                }
                catch (Exception ex)
                {
                    AppendLog($"Error while waiting for messages: {ex.Message}");
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Connection error: {ex.Message}");
            AppendLog($"Stack trace: {ex.StackTrace}");
        }
        finally
        {
            if (isConnected)
            {
                AppendLog("Connection closed");
                DisconnectFromServer();
            }
        }
    }

    private async Task FetchGameInfoAsync(DownloadInfo info)
    {
        try
        {
            // First fetch game info from Steam
            string url = $"https://store.steampowered.com/api/appdetails?appids={info.AppId}";
            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);
            using var doc = JsonDocument.Parse(response);
            var root = doc.RootElement;
            var appData = root.GetProperty(info.AppId);
            
            if (appData.GetProperty("success").GetBoolean())
            {
                var data = appData.GetProperty("data");
                string gameName = data.GetProperty("name").GetString() ?? "";
                string description = data.GetProperty("detailed_description").GetString() ?? "";
                string iconUrl = data.GetProperty("header_image").GetString() ?? "";

                if (this.InvokeRequired)
                {
                    this.Invoke(() =>
                    {
                        labelGameName.Text = gameName;
                        labelDescription.Text = description;
                        labelDescription.Visible = true;
                        panelDescriptionPlaceholder.Visible = false;
                    });
                }
                else
                {
                    labelGameName.Text = gameName;
                    labelDescription.Text = description;
                    labelDescription.Visible = true;
                    panelDescriptionPlaceholder.Visible = false;
                }

                if (!string.IsNullOrEmpty(iconUrl))
                {
                    try
                    {
                        var imgStream = await client.GetStreamAsync(iconUrl);
                        if (this.InvokeRequired)
                        {
                            this.Invoke(() =>
                            {
                                pictureBoxIcon.Image = Image.FromStream(imgStream);
                                pictureBoxIcon.Visible = true;
                                panelIconPlaceholder.Visible = false;
                            });
                        }
                        else
                        {
                            pictureBoxIcon.Image = Image.FromStream(imgStream);
                            pictureBoxIcon.Visible = true;
                            panelIconPlaceholder.Visible = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"Error loading image: {ex.Message}");
                        if (this.InvokeRequired)
                        {
                            this.Invoke(() =>
                            {
                                pictureBoxIcon.Image = null;
                                pictureBoxIcon.Visible = false;
                                panelIconPlaceholder.Visible = true;
                            });
                        }
                        else
                        {
                            pictureBoxIcon.Image = null;
                            pictureBoxIcon.Visible = false;
                            panelIconPlaceholder.Visible = true;
                        }
                    }
                }
                else
                {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(() =>
                        {
                            pictureBoxIcon.Image = null;
                            pictureBoxIcon.Visible = false;
                            panelIconPlaceholder.Visible = true;
                        });
                    }
                    else
                    {
                        pictureBoxIcon.Image = null;
                        pictureBoxIcon.Visible = false;
                        panelIconPlaceholder.Visible = true;
                    }
                }

                // Now download the manifest and depot key files
                await DownloadManifestAndKeyFilesAsync(info);
            }
            else
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(() =>
                    {
                        labelGameName.Text = "Game not found.";
                        labelDescription.Text = "";
                        labelDescription.Visible = false;
                        panelDescriptionPlaceholder.Visible = true;
                        pictureBoxIcon.Image = null;
                        pictureBoxIcon.Visible = false;
                        panelIconPlaceholder.Visible = true;
                    });
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
        }
        catch (Exception ex)
        {
            AppendLog($"Error fetching game info: {ex.Message}");
            if (this.InvokeRequired)
            {
                this.Invoke(() =>
                {
                    labelGameName.Text = "Error fetching game info.";
                    labelDescription.Text = "";
                    labelDescription.Visible = false;
                    panelDescriptionPlaceholder.Visible = true;
                    pictureBoxIcon.Image = null;
                    pictureBoxIcon.Visible = false;
                    panelIconPlaceholder.Visible = true;
                });
            }
            else
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
    }

    private async Task DownloadManifestAndKeyFilesAsync(DownloadInfo info)
    {
        try
        {
            AppendLog("Downloading manifest and depot key files...");
            Directory.CreateDirectory("manifests");

            // Download depot key file
            string keyUrl = $"{manifestRepoUrl}/raw/{info.AppId}/key.vdf";
            using var client = new HttpClient();
            string keyContent = await client.GetStringAsync(keyUrl);
            
            // Convert VDF format to required format
            var lines = new List<string>();
            var matches = Regex.Matches(keyContent, @"""(\d+)""\s*{\s*""DecryptionKey""\s+""([^""]+)""");
            foreach (Match match in matches)
            {
                string depotId = match.Groups[1].Value;
                string key = match.Groups[2].Value;
                lines.Add($"{depotId};{key}");
            }

            // Save depot key file
            string keyFilePath = Path.Combine("manifests", $"{info.AppId}.key");
            await File.WriteAllLinesAsync(keyFilePath, lines);
            AppendLog($"Saved depot keys to {keyFilePath}");

            // Download manifest file
            string manifestUrl = $"{manifestRepoUrl}/tree/{info.AppId}";
            string manifestResponse = await client.GetStringAsync(manifestUrl);
            
            // Extract manifest file URL for the specific depot
            var manifestMatches = Regex.Matches(manifestResponse, $@"href=""([^""]+{info.DepotId}_[^""]+\.manifest)""");
            if (manifestMatches.Count > 0)
            {
                string manifestFileName = Path.GetFileName(manifestMatches[0].Groups[1].Value);
                string manifestId = manifestFileName.Split('_')[1].Replace(".manifest", "");
                string manifestFileUrl = $"{manifestRepoUrl}/raw/refs/heads/{info.AppId}/{manifestFileName}";
                
                // Download manifest file
                var manifestBytes = await client.GetByteArrayAsync(manifestFileUrl);
                string manifestFilePath = Path.Combine("manifests", manifestFileName);
                await File.WriteAllBytesAsync(manifestFilePath, manifestBytes);
                AppendLog($"Downloaded manifest: {manifestFileName} ({manifestBytes.Length} bytes)");

                // Start download automatically
                if (this.InvokeRequired)
                {
                    this.Invoke(() => StartDownload(info.AppId, info.DepotId, manifestId, keyFilePath, manifestFilePath));
                }
                else
                {
                    StartDownload(info.AppId, info.DepotId, manifestId, keyFilePath, manifestFilePath);
                }
            }
            else
            {
                AppendLog($"No manifest file found for depot {info.DepotId}");
            }
        }
        catch (Exception ex)
        {
            AppendLog($"Error downloading files: {ex.Message}");
        }
    }

    private void StartDownload(string appId, string depotId, string manifestId, string depotKeysFile, string manifestFile)
    {
        if (isDownloading)
        {
            MessageBox.Show("A download is already in progress.", "Warning");
            return;
        }

        string args = $"-app {appId} -depotkeys \"{depotKeysFile}\" -depot {depotId} -manifest {manifestId} -manifestfile \"{manifestFile}\"";
        try
        {
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

    private void ClearGameInfo()
    {
        labelGameName.Text = "";
        labelDescription.Text = "";
        labelDescription.Visible = false;
        panelDescriptionPlaceholder.Visible = true;
        pictureBoxIcon.Image = null;
        pictureBoxIcon.Visible = false;
        panelIconPlaceholder.Visible = true;
        currentDownloadInfo = null;
    }

    private DownloadInfo? currentDownloadInfo;

    private void ClientForm_FormClosing(object? sender, FormClosingEventArgs e)
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
        DisconnectFromServer();
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
}

public class DownloadInfo
{
    public string AppId { get; set; } = "";
    public string DepotId { get; set; } = "";
    public string ManifestId { get; set; } = "";
    public string DepotKeysFile { get; set; } = "";
    public string ManifestFile { get; set; } = "";
} 