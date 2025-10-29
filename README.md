# Steam Depot Downloader GUI

A client-server application for downloading Steam game content using depot keys and manifests.

## Components

### Server Application
- Searches for games using Steam App IDs
- Displays game information and icons
- Manages depot keys and manifest files
- Downloads game content
- Sends download information to connected clients

### Client Application
- Connects to the server using IP:Port
- Receives game information and download details
- Downloads game content using the provided information

## Requirements
- .NET 8.0
- .NET 9.0
- Windows operating system
- DepotDownloaderMod.exe in the same directory as the applications

## Building
1. Open the solution in Visual Studio 2022
2. Build the solution (Ctrl+Shift+B)
3. The executables will be created in the respective project's bin/Debug or bin/Release folders

## Usage

### GUI
1. Run DepotDownloaderGUI.exe
2. Enter a Steam App ID and click Search
3. Click "Get depot keys" to download the necessary keys and manifests
4. Click "Download" and fill out depot id (for windows games it's just appid but ending number 0 should be 1
5. For sending to the client Click "Download" and select "Upload to Client" to send the information to a connected client

### Client
1. Run DepotDownloaderClient.exe
2. Enter the server's IP address and port (default: 127.0.0.1:5000)
3. Click Connect
4. Once connected, the game information will be displayed
5. Click Download to start downloading the game content

## Notes
- The server must be running before the client can connect
- Both applications require DepotDownloaderMod.exe to be present
- Make sure the necessary depot keys and manifest files are available before attempting to download
