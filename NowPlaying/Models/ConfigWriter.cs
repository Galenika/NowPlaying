using System.IO;
using System.Diagnostics;
using System.Linq;
using NowPlaying.Api.SpotifyResponses;

namespace NowPlaying.Models
{
    public class ConfigWriter
    {
        private string _writePath { get; set; }
        private string _writeConfigText { get; set; } = "say [Spotify] Now Playing: {0}";

        public ConfigWriter(string writePath, string writeConfigText = null)
        {
            _writeConfigText = writeConfigText ?? _writeConfigText;

            var process = Process.GetProcessesByName("hl2").FirstOrDefault();

            if (process != null)
                _writePath = process.MainModule.FileName.Replace("hl2.exe", "") + "tf\\cfg\\audio.cfg";
            else
                _writePath = writePath;

            EnsureCreatedDirectoriesAndConfigFile(_writePath);
        }

        private void EnsureCreatedDirectoriesAndConfigFile(string audioCfgFullPath)
        {
            var audioCfgDirectoryPath = _writePath.Remove(_writePath.Length - @"\audio.cfg".Length);
            Directory.CreateDirectory(audioCfgDirectoryPath);

            if (!File.Exists(_writePath))
                File.CreateText(_writePath).Dispose();
        }

        public void RewriteKeyBinding(CurrentTrackResponse track)
        {
            var bindingCommand = string.Format(_writeConfigText, track.FullName);

            RewriteKeyBinding(bindingCommand);
        }

        public void RewriteKeyBinding(string line)
        {
            using (var sw = new StreamWriter(_writePath))
                sw.WriteLine(line);
        }
    }
}