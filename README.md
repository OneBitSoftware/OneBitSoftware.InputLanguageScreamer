# 🎵 Input Language Screamer

**A Windows application that plays language-specific audio files when you switch input languages using Alt+Shift.**

NOTE: Automatically built with Claude Sonnet 4.0, no real coding.

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Windows](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)](https://www.microsoft.com/windows)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build](https://github.com/OneBitSoftware/OneBitSoftware.InputLanguageScreamer/actions/workflows/ci.yml/badge.svg)](https://github.com/OneBitSoftware/OneBitSoftware.InputLanguageScreamer/actions/workflows/ci.yml)
[![Release](https://github.com/OneBitSoftware/OneBitSoftware.InputLanguageScreamer/actions/workflows/build-and-release.yml/badge.svg)](https://github.com/OneBitSoftware/OneBitSoftware.InputLanguageScreamer/actions/workflows/build-and-release.yml)

## 🚀 Features

- **🎯 Global Language Detection**: Monitors language changes system-wide, regardless of which application has focus
- **🎵 Custom Audio Playback**: Plays language-specific MP3 files for each input language
- **🔇 Smart Fallback**: Falls back to system beep if audio files are missing
- **📱 System Tray Integration**: Runs invisibly in the background with system tray access
- **⚡ Lightweight**: Minimal resource usage with clean architecture
- **🛡️ Robust Error Handling**: Never crashes, always provides feedback

## 📋 Requirements

- **Operating System**: Windows 10/11
- **Framework**: .NET 9.0 Runtime (included in self-contained builds)
- **Audio**: Windows audio system

## 🎯 How It Works

1. **Detects Language Changes**: Uses Windows API low-level keyboard hook to monitor Alt+Shift combinations
2. **Identifies Current Language**: Determines the active input language from Windows
3. **Plays Corresponding Audio**: Maps language names to MP3 files and plays the appropriate sound
4. **Runs in Background**: Operates silently from the system tray

## 📁 Project Structure

```
OneBitSoftware.InputLanguageScreamer/
├── Desktop/
│   ├── Audio/                          # Audio files directory
│   │   ├── English.mp3                 # English language audio
│   │   └── Bulgarian.mp3               # Bulgarian language audio
│   ├── Program.cs                      # Application entry point
│   ├── LanguageMonitorApp.cs           # System tray application
│   ├── GlobalKeyboardHook.cs           # Windows API keyboard hook
│   ├── LanguageAudioPlayer.cs          # MP3 audio playback
│   └── Desktop.csproj                  # Project configuration
└── OneBitSoftware.InputLanguageScreamer.sln
```

## 🛠️ Installation

### Option 1: Download Release (Recommended)
1. Download the latest release from the [Releases](../../releases) page
2. Choose the appropriate package for your system:
   - **Windows x64** (Recommended): `InputLanguageScreamer-vX.X.X-win-x64.zip`
   - **Windows x86** (32-bit): `InputLanguageScreamer-vX.X.X-win-x86.zip`
   - **Windows ARM64**: `InputLanguageScreamer-vX.X.X-win-arm64.zip`
   - **Portable** (Minimal): `InputLanguageScreamer-vX.X.X-portable.zip`
3. Extract the ZIP file to your desired location
4. Add your MP3 audio files to the `Audio` folder
5. Run `Desktop.exe`

### Option 2: Build from Source
```bash
# Clone the repository
git clone https://github.com/OneBitSoftware/OneBitSoftware.InputLanguageScreamer.git
cd OneBitSoftware.InputLanguageScreamer

# Build the project
dotnet build --configuration Release

# Publish as single executable
dotnet publish Desktop/Desktop.csproj --configuration Release --output ./publish
```

## 🚀 Automated Releases

This project uses GitHub Actions for automated building and releasing:

### Creating a New Release
1. **Tag a new version**: `git tag v1.0.1 && git push origin v1.0.1`
2. **Automatic build**: GitHub Actions will automatically build for all platforms
3. **Release creation**: A new GitHub release will be created with all packages
4. **Multi-platform support**: Builds for Windows x64, x86, and ARM64

### Continuous Integration
- **Every push/PR**: Automatic build verification
- **Quality checks**: Code builds successfully on Windows
- **Status badges**: Build status visible in README

## 🎵 Adding Audio Files

1. Create MP3 files named after your input languages:
   - `English.mp3` for English
   - `Bulgarian.mp3` for Bulgarian
   - `Spanish.mp3` for Spanish
   - etc.

2. Place them in the `Audio` folder next to the executable

3. The application will automatically detect and play the appropriate file when you switch languages

### Supported Audio Formats
- **Primary**: MP3 (recommended)
- **Fallback**: System beep if MP3 files are missing

## 🚀 Usage

1. **Start the Application**: Run `Desktop.exe`
2. **System Tray**: The app will appear in your system tray
3. **Switch Languages**: Use Alt+Shift to change input languages
4. **Hear Audio**: The corresponding MP3 file will play
5. **Exit**: Right-click the system tray icon and select "Exit"

## ⚙️ Configuration

### Language Mapping
The application automatically maps Windows input languages to audio files:

| Windows Language | Audio File |
|------------------|------------|
| English (United States) | `English.mp3` |
| Bulgarian | `Bulgarian.mp3` |
| Spanish | `Spanish.mp3` |
| French | `French.mp3` |

### Custom Languages
To add support for additional languages:
1. Add the corresponding MP3 file to the `Audio` folder
2. Name it using the English name of the language
3. The application will automatically detect it

## 🏗️ Architecture

### Core Components

- **`LanguageMonitorApp`**: Main application context managing system tray
- **`GlobalKeyboardHook`**: Windows API integration for global keyboard monitoring
- **`LanguageAudioPlayer`**: MP3 playback using NAudio library
- **`Program`**: Application entry point

### Dependencies

- **NAudio**: High-quality audio playback library
- **.NET Windows Forms**: System tray and UI components
- **Windows API**: Low-level keyboard hook functionality

## 🔧 Development

### Prerequisites
- Visual Studio 2022 or VS Code
- .NET 9.0 SDK
- Windows 10/11

### Building
```bash
# Debug build
dotnet build

# Release build
dotnet build --configuration Release

# Run locally
dotnet run --project Desktop/Desktop.csproj
```

### Testing
1. Build the application
2. Add test MP3 files to the Audio folder
3. Run the application
4. Switch input languages using Alt+Shift
5. Verify audio playback

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **NAudio**: Excellent .NET audio library
- **Microsoft**: Windows API documentation and .NET framework
- **Community**: Feedback and feature suggestions

## 📞 Support

- **Issues**: [GitHub Issues](../../issues)
- **Discussions**: [GitHub Discussions](../../discussions)

## 🔄 Changelog

### v1.0.0 (2025-01-16)
- Initial release
- Global language change detection
- MP3 audio playback support
- System tray integration
- Support for English and Bulgarian languages

---

**Made with ❤️ by OneBit Software**