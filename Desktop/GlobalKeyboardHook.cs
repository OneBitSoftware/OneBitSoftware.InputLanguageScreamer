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
    private const int WM_KEYDOWN = 0x0100;      // Key down message
    private const int WM_KEYUP = 0x0101;        // Key up message
    private const int WM_SYSKEYDOWN = 0x0104;   // System key down (Alt combinations)
    private const int WM_SYSKEYUP = 0x0105;     // System key up (Alt combinations)
    private const int VK_LMENU = 0xA4;          // Left Alt key
    private const int VK_RMENU = 0xA5;          // Right Alt key
    private const int VK_LSHIFT = 0xA0;         // Left Shift key
    private const int VK_RSHIFT = 0xA1;         // Right Shift key
    private const int VK_SHIFT = 0x10;          // Generic Shift key

    // Static fields for the hook
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
    private static uint _currentInputLanguage = 0;
    private static Action<string>? _onLanguageChange;
    private static System.Threading.Timer? _languageCheckTimer;
    private static bool _leftAltPressed = false;
    private static bool _leftShiftPressed = false;

    /// <summary>
    /// Delegate for the low-level keyboard procedure
    /// </summary>
    public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Initializes the global keyboard hook with a callback for language changes
    /// </summary>
    /// <param name="onLanguageChange">Action to execute when language changes, receives the new language name</param>
    public GlobalKeyboardHook(Action<string> onLanguageChange)
    {
        _onLanguageChange = onLanguageChange;
        
        // Get the initial keyboard layout for the foreground window's thread
        var foregroundWindow = GetForegroundWindow();
        var threadId = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
        var keyboardLayout = GetKeyboardLayout(threadId);
        _currentInputLanguage = (uint)((long)keyboardLayout & 0xFFFF);
        
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
    /// Monitors for potential language switching keys and checks for language changes
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
            
            // Check if this is a key press or release event
            bool isKeyDown = wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN;
            bool isKeyUp = wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP;
            
            // Track Left Alt and Left Shift key states
            if (vkCode == VK_LMENU)
            {
                if (isKeyDown)
                {
                    _leftAltPressed = true;
                    Debug.WriteLine("Left Alt pressed");
                }
                else if (isKeyUp)
                {
                    _leftAltPressed = false;
                    Debug.WriteLine("Left Alt released");
                    
                    // Check for language change when Alt is released if Shift was also pressed
                    if (_leftShiftPressed)
                    {
                        Debug.WriteLine("Alt+Shift combination detected (Alt released) - checking for language change");
                        ScheduleLanguageCheck();
                    }
                }
            }
            else if (vkCode == VK_LSHIFT)
            {
                if (isKeyDown)
                {
                    _leftShiftPressed = true;
                    Debug.WriteLine("Left Shift pressed");
                }
                else if (isKeyUp)
                {
                    _leftShiftPressed = false;
                    Debug.WriteLine("Left Shift released");
                    
                    // Check for language change when Shift is released if Alt was also pressed
                    if (_leftAltPressed)
                    {
                        Debug.WriteLine("Alt+Shift combination detected (Shift released) - checking for language change");
                        ScheduleLanguageCheck();
                    }
                }
            }
        }

        // Pass the hook information to the next hook procedure
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    /// <summary>
    /// Schedules a single delayed language check
    /// </summary>
    private static void ScheduleLanguageCheck()
    {
        // Cancel any existing timer to prevent duplicates
        _languageCheckTimer?.Dispose();
        
        // Schedule a single check after a delay to allow the language change to complete
        _languageCheckTimer = new System.Threading.Timer(
            callback: _ => CheckLanguageChange(),
            state: null,
            dueTime: TimeSpan.FromMilliseconds(250),
            period: Timeout.InfiniteTimeSpan
        );
    }

    /// <summary>
    /// Checks if the input language has changed and triggers the callback if it has
    /// </summary>
    private static void CheckLanguageChange()
    {
        try
        {
            // Get the keyboard layout for the foreground window's thread instead of current thread
            var foregroundWindow = GetForegroundWindow();
            var threadId = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
            var keyboardLayout = GetKeyboardLayout(threadId);
            var newInputLanguage = (uint)((long)keyboardLayout & 0xFFFF);

            // Get language name for debugging
            string currentLangName = GetLanguageName(_currentInputLanguage);
            string newLangName = GetLanguageName(newInputLanguage);

            Debug.WriteLine($"Current: {currentLangName} (0x{_currentInputLanguage:X4}), New: {newLangName} (0x{newInputLanguage:X4}), Thread: {threadId}, Layout: 0x{(long)keyboardLayout:X8}");

            if (newInputLanguage != _currentInputLanguage)
            {
                Debug.WriteLine($"Language changed from {currentLangName} to {newLangName}");
                _currentInputLanguage = newInputLanguage;
                _onLanguageChange?.Invoke(newLangName);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error checking language change: {ex.Message}");
            // Silently ignore errors to prevent disrupting the hook
        }
    }

    /// <summary>
    /// Gets a readable language name from language ID for debugging and audio file matching
    /// </summary>
    private static string GetLanguageName(uint langId)
    {
        return langId switch
        {
            0x0409 => "English",
            0x0809 => "English", 
            0x0402 => "Bulgarian",
            0x0407 => "German",
            0x040C => "French",
            0x0410 => "Italian",
            0x0C0A => "Spanish",
            0x0419 => "Russian",
            0x041F => "Turkish",
            _ => $"Unknown"
        };
    }

    /// <summary>
    /// Disposes of the keyboard hook resources
    /// </summary>
    public void Dispose()
    {
        UnhookWindowsHookEx(_hookID);
        _languageCheckTimer?.Dispose();
        _languageCheckTimer = null;
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

    /// <summary>
    /// Retrieves the active input locale identifier (keyboard layout) for the specified thread
    /// </summary>
    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint idThread);

    /// <summary>
    /// Retrieves a handle to the foreground window
    /// </summary>
    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    /// <summary>
    /// Retrieves the identifier of the thread that created the specified window
    /// </summary>
    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);
}