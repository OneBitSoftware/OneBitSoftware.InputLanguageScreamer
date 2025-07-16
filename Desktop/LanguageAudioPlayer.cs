// Input Language Screamer - Plays language-specific sounds when Windows input language changes
// Audio player component for MP3 file playback
namespace OneBitSoftware.InputLanguageScreamer.Desktop;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using NAudio.Wave;

/// <summary>
/// Audio player for MP3 files using NAudio
/// Handles playing language-specific audio files when input language changes
/// </summary>
public class LanguageAudioPlayer
{
    private readonly string audioDirectory;
    private IWavePlayer? wavePlayer;
    private AudioFileReader? audioFileReader;
    private readonly Dictionary<string, string> audioFileCache;

    /// <summary>
    /// Initializes the audio player with the specified audio directory
    /// </summary>
    /// <param name="audioDirectory">Directory containing the MP3 audio files</param>
    public LanguageAudioPlayer(string audioDirectory)
    {
        this.audioDirectory = audioDirectory;
        this.audioFileCache = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        CacheAudioFiles();
    }
    
    /// <summary>
    /// Caches all available audio files at startup to avoid disk operations during language changes
    /// </summary>
    private void CacheAudioFiles()
    {
        try
        {
            if (Directory.Exists(audioDirectory))
            {
                var audioFiles = Directory.GetFiles(audioDirectory, "*.mp3");
                foreach (var file in audioFiles)
                {
                    var languageName = Path.GetFileNameWithoutExtension(file);
                    audioFileCache[languageName] = file;
                }
            }
        }
        catch (Exception)
        {
            // Silently handle any errors during caching
        }
    }

    /// <summary>
    /// Plays the appropriate audio file for the current input language
    /// Maps language names to corresponding MP3 files
    /// </summary>
    public void PlayLanguageAudio()
    {
        try
        {
            // Stop any currently playing audio
            StopCurrentAudio();

            // Get current input language
            var currentLanguage = InputLanguage.CurrentInputLanguage;
            var languageName = GetLanguageDisplayName(currentLanguage);
            
            // Try to find matching audio file
            var audioFile = FindAudioFileForLanguage(languageName);
            
            if (audioFile != null && File.Exists(audioFile))
            {
                // Play the language-specific audio file
                PlayAudioFile(audioFile);
            }
            else
            {
                // Fallback to system beep if no audio file found
                SystemSounds.Beep.Play();
            }
        }
        catch (Exception)
        {
            // Fallback to system beep on any error
            SystemSounds.Beep.Play();
        }
    }

    /// <summary>
    /// Gets a simplified display name for the language
    /// </summary>
    /// <param name="language">Input language to get name for</param>
    /// <returns>Simplified language name</returns>
    private string GetLanguageDisplayName(InputLanguage language)
    {
        // Extract language name from culture info
        var cultureName = language.Culture.EnglishName;
        var layoutName = language.LayoutName;
        
        // Debug info - can be removed after fixing
        Console.WriteLine($"Culture: {cultureName}, Layout: {layoutName}, LCID: {language.Culture.LCID}");
        
        // Handle Bulgarian specifically - check both culture name and layout
        if (cultureName.Contains("Bulgarian") || layoutName.Contains("Bulgarian"))
            return "Bulgarian";
            
        // Handle English
        if (cultureName.Contains("English"))
            return "English";
        
        // For other languages, try to extract the first word
        var firstWord = cultureName.Split(' ')[0];
        return firstWord;
    }

    /// <summary>
    /// Finds the audio file that matches the given language name
    /// </summary>
    /// <param name="languageName">Name of the language to find audio for</param>
    /// <returns>Path to the audio file, or null if not found</returns>
    private string? FindAudioFileForLanguage(string languageName)
    {
        // Use the cached audio file if available
        if (audioFileCache.TryGetValue(languageName, out var audioFile))
        {
            return audioFile;
        }
        
        return null;
    }

    /// <summary>
    /// Plays the specified audio file using NAudio
    /// </summary>
    /// <param name="audioFilePath">Path to the MP3 file to play</param>
    private void PlayAudioFile(string audioFilePath)
    {
        // Create audio file reader
        audioFileReader = new AudioFileReader(audioFilePath);
        
        // Create wave player
        wavePlayer = new WaveOutEvent();
        wavePlayer.Init(audioFileReader);
        
        // Handle playback completion
        wavePlayer.PlaybackStopped += (sender, e) =>
        {
            StopCurrentAudio();
        };
        
        // Start playback
        wavePlayer.Play();
    }

    /// <summary>
    /// Stops any currently playing audio and disposes resources
    /// </summary>
    private void StopCurrentAudio()
    {
        wavePlayer?.Stop();
        wavePlayer?.Dispose();
        audioFileReader?.Dispose();
        wavePlayer = null;
        audioFileReader = null;
    }

    /// <summary>
    /// Disposes of audio player resources
    /// </summary>
    public void Dispose()
    {
        StopCurrentAudio();
    }
}