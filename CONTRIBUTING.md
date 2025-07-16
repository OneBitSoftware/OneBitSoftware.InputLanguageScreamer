# Contributing to Input Language Screamer

Thank you for your interest in contributing to Input Language Screamer! We welcome contributions from the community.

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022 or VS Code
- .NET 9.0 SDK
- Windows 10/11 for testing
- Git

### Setting Up Development Environment

1. **Fork the repository**
   ```bash
   # Fork on GitHub, then clone your fork
   git clone https://github.com/YOUR_USERNAME/OneBitSoftware.InputLanguageScreamer.git
   cd OneBitSoftware.InputLanguageScreamer
   ```

2. **Build the project**
   ```bash
   dotnet build
   ```

3. **Run the application**
   ```bash
   dotnet run --project Desktop/Desktop.csproj
   ```

## 🛠️ Development Guidelines

### Code Style
- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and single-purpose

### Architecture
- **`LanguageMonitorApp`**: System tray and application lifecycle
- **`GlobalKeyboardHook`**: Windows API keyboard monitoring
- **`LanguageAudioPlayer`**: Audio file management and playback
- **`Program`**: Application entry point

### Adding New Features

1. **Create a feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes**
   - Follow existing code patterns
   - Add appropriate error handling
   - Include XML documentation

3. **Test your changes**
   - Build the project: `dotnet build`
   - Test manually with different languages
   - Ensure no regressions

4. **Commit and push**
   ```bash
   git add .
   git commit -m "Add: Your feature description"
   git push origin feature/your-feature-name
   ```

5. **Create a Pull Request**

## 🎵 Adding Language Support

To add support for a new language:

1. **Identify the language name**
   - Check how Windows reports the language in `InputLanguage.Culture.EnglishName`
   - Example: "Spanish (Spain)" → "Spanish"

2. **Update language mapping** (if needed)
   - Modify `GetLanguageDisplayName()` in `LanguageAudioPlayer.cs`
   - Add special handling for complex language names

3. **Test with audio file**
   - Create a test MP3 file named after the language
   - Place it in the Audio folder
   - Test language switching

## 🐛 Bug Reports

When reporting bugs, please include:

- **Operating System**: Windows version
- **Application Version**: Version number
- **Steps to Reproduce**: Clear steps to reproduce the issue
- **Expected Behavior**: What should happen
- **Actual Behavior**: What actually happens
- **Audio Files**: Information about your MP3 files (if relevant)

## 💡 Feature Requests

We welcome feature requests! Please:

- Check existing issues first
- Describe the use case clearly
- Explain why the feature would be valuable
- Consider implementation complexity

## 🔧 Common Development Tasks

### Adding a New Audio Format
1. Update `LanguageAudioPlayer.cs`
2. Modify `FindAudioFileForLanguage()` method
3. Test with the new format

### Improving Language Detection
1. Update `GetLanguageDisplayName()` method
2. Add new language mappings
3. Test with various Windows language settings

### Enhancing System Tray
1. Modify `LanguageMonitorApp.cs`
2. Update `InitializeSystemTray()` method
3. Test system tray interactions

## 📋 Pull Request Process

1. **Ensure your PR**:
   - Has a clear title and description
   - References any related issues
   - Includes appropriate tests
   - Follows coding standards

2. **PR Review Process**:
   - Maintainers will review your code
   - Address any feedback promptly
   - Make requested changes
   - Ensure CI passes

3. **After Approval**:
   - PR will be merged by maintainers
   - Your contribution will be credited

## 🏷️ Versioning

We use [Semantic Versioning](https://semver.org/):
- **MAJOR**: Breaking changes
- **MINOR**: New features (backward compatible)
- **PATCH**: Bug fixes (backward compatible)

## 📞 Getting Help

- **GitHub Issues**: For bugs and feature requests
- **GitHub Discussions**: For questions and general discussion
- **Email**: development@onebitsoftware.com

## 🙏 Recognition

Contributors will be:
- Listed in the project contributors
- Mentioned in release notes
- Credited in the README

Thank you for contributing to Input Language Screamer! 🎵