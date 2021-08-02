using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;

namespace NoVRCJustNeosVR
{
    public static class BuildInfo
    {
        public const string Name = "NoVRCJustNeosVR";
        public const string Description = "VRChat gay, play NeosVR instead";
        public const string Author = "Not So Funny Lily";
        public const string Company = null;
        public const string Version = "2.0.0";
        public const string DownloadLink = null;
    }

    public class NoVRCJustNeosVR : MelonMod
    {
        public override void OnApplicationStart()
        {
            try {
                string local = PathLogic.GetInstallLocation();
                Process p = new();
                p.StartInfo = new ProcessStartInfo($"{local}\\{PathLogic.AppFileName}", string.Empty){
                    UseShellExecute = false,
                    WorkingDirectory = local,
                };
                p.StartInfo.Environment["SteamAppId"] = PathLogic.SteamAppId.ToString();
                p.Start();
            }
            catch (Exception) { new Exception(); }
            Process.GetCurrentProcess().Kill();
        }
    }

    // Came from https://github.com/Slaynash/VRChatModInstaller/blob/074fe127b26945c622c0afcd9941aa8a9b0632b2/VRCModManager/Dependencies/SteamFinder.cs
    class SteamFinder
    {
        public string SteamPath { get; private set; }
        public string[] Libraries { get; private set; }

        /// <summary>
        /// Tries to find the Steam folder and its libraries on the system.
        /// </summary>
        /// <returns>Returns true if a valid Steam installation folder path was found.</returns>
        public bool FindSteam()
        {
            SteamPath = null;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    SteamPath = FindWindowsSteamPath();
                    break;
                default:
                    if (IsUnix())
                        SteamPath = FindUnixSteamPath();
                    break;
            }

            if (SteamPath == null)
                return false;

            return FindLibraries();
        }

        /// <summary>
        /// Retrieves the game folder by reading the game's Steam manifest. The game needs to be marked as installed on Steam.
        /// <para>Returns null if not found.</para>
        /// </summary>
        /// <param name="appId">The game's app id on Steam.</param>
        /// <returns>The path to the game folder.</returns>
        public string FindGameFolder(int appId)
        {
            if (Libraries == null)
                throw new InvalidOperationException("Steam must be found first.");

            foreach (var library in Libraries)
            {
                var gameManifestPath = GetManifestFilePath(library, appId);
                if (gameManifestPath == null)
                    continue;

                var gameFolderName = ReadInstallDirFromManifest(gameManifestPath);
                if (gameFolderName == null)
                    continue;

                return Path.Combine(library, "common", gameFolderName);
            }

            return null;
        }

        /// <summary>
        /// Searches for a game directory that has the specified name in known libraries.
        /// </summary>
        /// <param name="gameFolderName">The game's folder name inside the Steam library.</param>
        /// <returns>The game folders path in the libraries.</returns>
        public IEnumerable<string> FindGameFolders(string gameFolderName)
        {
            if (Libraries == null)
                throw new InvalidOperationException("Steam must be found first.");

            gameFolderName = gameFolderName.ToLowerInvariant();

            foreach (var library in Libraries)
            {
                var folder = Directory.EnumerateDirectories(library)
                    .FirstOrDefault(f => Path.GetFileName(f).ToLowerInvariant() == gameFolderName);

                if (folder != null)
                    yield return folder;
            }
        }

        bool FindLibraries()
        {
            var steamLibraries = new List<string>();
            var steamDefaultLibrary = Path.Combine(SteamPath, "steamapps");
            if (!Directory.Exists(steamDefaultLibrary))
                return false;

            steamLibraries.Add(steamDefaultLibrary);

            /*
             * Get library folders paths from libraryfolders.vdf
             *
             * Libraries are listed like this:
             *     "id"   "library folder path"
             *
             * Examples:
             *     "1"   "D:\\Games\\SteamLibraryOnD"
             *     "2"   "E:\\Games\\steam_games"
             */
            var regex = new Regex(@"""\d+""\s+""(.+)""");
            var libraryFoldersFilePath = Path.Combine(steamDefaultLibrary, "libraryfolders.vdf");
            if (File.Exists(libraryFoldersFilePath))
            {
                foreach (var line in File.ReadAllLines(libraryFoldersFilePath))
                {
                    var match = regex.Match(line);
                    if (!match.Success)
                        continue;

                    var libPath = match.Groups[1].Value;
                    libPath = libPath.Replace("\\\\", "\\"); // unescape the backslashes
                    libPath = Path.Combine(libPath, "steamapps");
                    if (Directory.Exists(libPath))
                        steamLibraries.Add(libPath);
                }
            }

            Libraries = steamLibraries.ToArray();
            return true;
        }

        static string GetManifestFilePath(string libraryPath, int appId)
        {
            var manifestPath = Path.Combine(libraryPath, $"appmanifest_{appId}.acf");
            if (File.Exists(manifestPath))
                return manifestPath;
            else
                return null;
        }

        static string ReadInstallDirFromManifest(string manifestFilePath)
        {
            var regex = new Regex(@"""installdir""\s+""(.+)""");
            foreach (var line in File.ReadAllLines(manifestFilePath))
            {
                var match = regex.Match(line);
                if (!match.Success)
                    continue;

                return match.Groups[1].Value;
            }

            return null;
        }

        static string FindWindowsSteamPath()
        {
            var regPath = Environment.Is64BitOperatingSystem
                 ? @"SOFTWARE\Wow6432Node\Valve\Steam"
                 : @"SOFTWARE\Valve\Steam";
            var subRegKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regPath);
            var path = subRegKey?.GetValue("InstallPath").ToString()
                .Replace('/', '\\'); // not actually required, just for consistency's sake

            if (Directory.Exists(path))
                return path;
            else
                return null;
        }

        static string FindUnixSteamPath()
        {
            string path = null;
            if (Directory.Exists(path = GetDefaultLinuxSteamPath())
                || Directory.Exists(path = GetDefaultMacOsSteamPath()))
            {
                return path;
            }

            return null;
        }

        static string GetDefaultLinuxSteamPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                ".local/share/Steam/"
            );
        }

        static string GetDefaultMacOsSteamPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "Library/Application Support/Steam"
            );
        }

        // https://stackoverflow.com/questions/5116977
        static bool IsUnix()
        {
            var p = (int)Environment.OSVersion.Platform;
            return p == 4 || p == 6 || p == 128;
        }
    }

    // Came from https://github.com/Slaynash/VRChatModInstaller/blob/074fe127b26945c622c0afcd9941aa8a9b0632b2/VRCModManager/Core/PathLogic.cs
    internal class PathLogic
    {
        public static string installPath;
        //public Platform platform;

        internal const int SteamAppId = 740250;
        internal const string AppFileName = "Neos.exe";
        private const string AppName = "NeosVR";

        public static string GetInstallLocation()
        {
            string steam = GetSteamLocation();
            if (steam != null)
            {
                if (Directory.Exists(steam))
                {
                    if (File.Exists(Path.Combine(steam, AppFileName)))
                    {
                        //platform = Platform.Steam;
                        installPath = steam;
                        return steam;
                    }
                }
            }
            string fallback = GetFallbackDirectory();
            installPath = fallback;
            return fallback;
        }

        private static string GetFallbackDirectory()
        {
            MessageBox.Show($"We couldn't seem to find your {AppName} installation, please press \"OK\" and point us to it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return NotFoundHandler();
        }

        public static string GetSteamLocation()
        {
            try
            {
                var steamFinder = new SteamFinder();
                if (!steamFinder.FindSteam())
                    return null;

                return steamFinder.FindGameFolder(SteamAppId);
            }
            catch {return null;}

        }

        private static string NotFoundHandler()
        {
            bool found = false;
            while (found == false)
            {
                using (var folderDialog = new OpenFileDialog())
                {
                    folderDialog.Title = $"Select {AppFileName}";
                    folderDialog.FileName = $"{AppFileName}";
                    folderDialog.Filter = $"{AppName} Executable|{AppFileName}";
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = folderDialog.FileName;
                        if (path.Contains($"{AppFileName}"))
                        {
                            string pathedited = path.Replace($@"\{AppFileName}", "");
                            installPath = pathedited;
                            return pathedited;
                        }
                        else
                        {
                            MessageBox.Show($"The directory you selected doesn't contain {AppFileName}! please try again!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return string.Empty;
        }
    }
}