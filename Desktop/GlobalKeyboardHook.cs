// Input Language Screamer - Plays language-specific sounds when Windows input language changes
// Global keyboard hook component for system-wide language change detection
namespace OneBitSoftware.InputLanguageScreamer.Desktop;

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/// <summary>
/// Global keyboard hook that monitors for input language changes system-wide.
/// Uses Windows API low-level keyboard hook to detect Alt key releases and check for language changes.
/// </summary>
public class GlobalKeyboardHook
{
    // Windows API constants for keyboard hook
    private const int WH_KEYBOARD_LL = 13;      // Low-level keyboard input hook
    private const int WM_KEYUP = 0x0101;        // Key up message
    private const int VK_LMENU = 0xA4;          // Left Alt key
    private const int VK_RMENU = 0xA5;          // Right Alt key

    // Static fields for the hook
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
    private static InputLanguage? _currentLanguage;
    private static Action? _onLanguageChange;

    /// <summary>
    /// Delegate for the low-level keyboard procedure
    /// </summary>
    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Initializes the global keyboard hook with a callback for language changes
    /// </summary>
    /// <param name="onLanguageChange">Action to execute when language changes</param>
    public GlobalKeyboardHook(Action onLanguageChange)
    {
        _onLanguageChange = onLanguageChange;
        _currentLanguage = InputLanguage.CurrentInputLanguage;
        _hookID = SetHook(_proc);
    }

    /// <summary>
    /// Sets up the low-level keyboard hook using Windows API
    /// </summary>
    /// <param name="proc">The callback procedure for keyboard events</param>
    /// <returns>Handle to the hook</returns>
    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule? curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule?.ModuleName ?? ""), 0);
        }
    }

    /// <summary>
    /// Callback function that processes keyboard events from the hook
    /// Monitors for Alt key releases and checks for language changes
    /// </summary>
    /// <param name="nCode">Hook code</param>
    /// <param name="wParam">Message identifier</param>
    /// <param name="lParam">Pointer to keyboard input structure</param>
    /// <returns>Result of calling next hook</returns>
    private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        // Only process if hook code is valid
        if (nCode >= 0)
        {
            // Get the virtual key code from the keyboard input structure
            int vkCode = Marshal.ReadInt32(lParam);
            
            // Check if this is a key release event
            if (wParam == (IntPtr)WM_KEYUP)
            {
                // Check if Alt key was released (left or right Alt)
                if (vkCode == VK_LMENU || vkCode == VK_RMENU)
                {
                    // Check for language change when Alt is released
                    // This catches Alt+Shift language switching
                    CheckLanguageChange();
                }
            }
        }

        // Pass the hook information to the next hook procedure
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    /// <summary>
    /// Checks if the input language has changed and triggers the callback if it has
    /// </summary>
    private static void CheckLanguageChange()
    {
        var newLanguage = InputLanguage.CurrentInputLanguage;
        if (newLanguage != _currentLanguage)
        {
            _currentLanguage = newLanguage;
            _onLanguageChange?.Invoke();
        }
    }

    /// <summary>
    /// Disposes of the keyboard hook resources
    /// </summary>
    public void Dispose()
    {
        UnhookWindowsHookEx(_hookID);
    }

    // Windows API function imports for keyboard hook functionality
    
    /// <summary>
    /// Installs an application-defined hook procedure into a hook chain
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    /// <summary>
    /// Uninstalls a hook procedure installed by SetWindowsHookEx
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    /// <summary>
    /// Passes hook information to the next hook procedure in the current hook chain
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Retrieves a module handle for the specified module
    /// </summary>
    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);
}