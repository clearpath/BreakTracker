using Newtonsoft.Json;
using System.IO;
using System.Windows.Forms;

namespace TheProject
{
	class Config
	{
		static FileSystemWatcher fileSystemWatcher;

		static Config()
		{
			var directoryName = Path.GetDirectoryName(Application.ExecutablePath);
			fileSystemWatcher = new FileSystemWatcher(directoryName, "*.json");
			fileSystemWatcher.EnableRaisingEvents = true;
			fileSystemWatcher.Changed += fileSystemWatcher_Changed;
		}

		private Config()
		{

		}

		public const string FILE_NAME = "config.json";

		static void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			if (e.Name == FILE_NAME)
			{
				instance = null;
			}
		}

		private static Config instance;

		public static Config Instance
		{
			get
			{
				if (instance == null)
				{
					var s = File.ReadAllText(FILE_NAME);
					instance = JsonConvert.DeserializeObject<Config>(s);
				}

				return instance;
			}
		}

		public bool NotifyOnBreak = false;
		public int MiniBreakIntervalInMinutes = 5;
		public int MiniBreakLengthInSeconds = 30;
		public int BigBreakIntervalInMinutes = 45;
		public int BigBreakLengthInSeconds = 240;
	}
}