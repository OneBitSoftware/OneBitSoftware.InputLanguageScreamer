// Input Language Screamer - Plays language-specific sounds when Windows input language changes
// Main program entry point
namespace OneBitSoftware.InputLanguageScreamer.Desktop;

using System;
using System.Windows.Forms;

/// <summary>
/// Main program entry point for Keyboard Screamer
/// Initializes and runs the language monitoring application
/// </summary>
static class Program
{
    /// <summary>
    /// The main entry point for the application
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Enable visual styles for Windows Forms
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        
        // Run the language monitor application in system tray
        Application.Run(new LanguageMonitorApp());
    }
}