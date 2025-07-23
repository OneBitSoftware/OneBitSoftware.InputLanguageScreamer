// Input Language Screamer - Plays language-specific sounds when Windows input language changes
// Main application context for system tray functionality
namespace OneBitSoftware.InputLanguageScreamer.Desktop;

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

/// <summary>
/// System tray application that monitors for input language changes
/// Runs invisibly in the background and provides system tray access
/// </summary>
public class LanguageMonitorApp : ApplicationContext
{
    private NotifyIcon? notifyIcon;
    private GlobalKeyboardHook? keyboardHook;
    private LanguageAudioPlayer? audioPlayer;

    /// <summary>
    /// Initializes the language monitor application
    /// Sets up system tray icon and global keyboard hook
    /// </summary>
    public LanguageMonitorApp()
    {
        InitializeSystemTray();
        InitializeKeyboardHook();
    }

    /// <summary>
    /// Sets up the system tray icon with context menu
    /// </summary>
    private void InitializeSystemTray()
    {
        // Create system tray icon
        notifyIcon = new NotifyIcon()
        {
            Icon = SystemIcons.Application,
            Visible = true,
            Text = "Input Language Screamer - Language Change Monitor"
        };

        // Create context menu for system tray
        var contextMenu = new ContextMenuStrip();
        
        // Add exit option to context menu
        var exitItem = new ToolStripMenuItem("Exit Input Language Screamer");
        exitItem.Click += (sender, e) => 
        {
            ExitApplication();
        };
        contextMenu.Items.Add(exitItem);
        
        notifyIcon.ContextMenuStrip = contextMenu;
    }

    /// <summary>
    /// Initializes the global keyboard hook for language change detection
    /// </summary>
    private void InitializeKeyboardHook()
    {
        // Initialize audio player with the Audio directory
        var audioDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio");
        audioPlayer = new LanguageAudioPlayer(audioDirectory);
        
        // Set up keyboard hook to play language-specific audio on language change
        keyboardHook = new GlobalKeyboardHook((languageName) =>
        {
            // Play language-specific MP3 audio when language changes
            audioPlayer.PlayLanguageAudio(languageName);
        });
    }

    /// <summary>
    /// Safely exits the application and cleans up resources
    /// </summary>
    private void ExitApplication()
    {
        // Clean up resources
        keyboardHook?.Dispose();
        audioPlayer?.Dispose();
        notifyIcon?.Dispose();
        
        // Exit the application
        Application.Exit();
    }

    /// <summary>
    /// Disposes of application resources
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            keyboardHook?.Dispose();
            audioPlayer?.Dispose();
            notifyIcon?.Dispose();
        }
        base.Dispose(disposing);
    }
}