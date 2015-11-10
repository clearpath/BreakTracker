using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;

namespace TheProject
{
	class Config
	{
		private Config()
		{

		}

		public const string FILE_NAME = "config.json";

		static FileSystemWatcher fileSystemWatcher;

		public static void Initialize()
		{
			var directoryName = Path.GetDirectoryName(Application.ExecutablePath);
			fileSystemWatcher = new FileSystemWatcher(directoryName, "*.json");
			fileSystemWatcher.EnableRaisingEvents = true;
			fileSystemWatcher.Changed += fileSystemWatcher_Changed;
			ReadSettings();
		}

		static void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (e.Name == FILE_NAME)
			{
				ReadSettings();
			}
		}

		private static void ReadSettings()
		{
			var s = File.ReadAllText(FILE_NAME);
			Instance = JsonConvert.DeserializeObject<Config>(s);
		}

		public static Config Instance { get; private set; }

		public int MiniBreakIntervalInMinutes = 5;
		public int MiniBreakLengthInSeconds = 30;
		public int BigBreakIntervalInMinutes = 45;
		public int BigBreakLengthInSeconds = 240;
	}
}