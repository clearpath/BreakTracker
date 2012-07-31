using System.Configuration;

namespace TheProject
{
    internal static class ConfigHelper
    {
        private static int? _MiniBreakInterval;
        public static int MiniBreakInterval
        {
            get
            {
                if (_MiniBreakInterval == null)
                    _MiniBreakInterval = int.Parse(ConfigurationManager.AppSettings["mini_break_interval"]);

                return _MiniBreakInterval.Value;
            }
        }

        private static int? _MiniBreakLength;
        public static int MiniBreakLength
        {
            get
            {
                if (_MiniBreakLength == null)
                    _MiniBreakLength = int.Parse(ConfigurationManager.AppSettings["mini_break_length"]);

                return _MiniBreakLength.Value;
            }
        }

        private static int? _BigBreakInterval;
        public static int BigBreakInterval
        {
            get
            {
                if (_BigBreakInterval == null)
                    _BigBreakInterval = int.Parse(ConfigurationManager.AppSettings["big_break_interval"]);

                return _BigBreakInterval.Value;
            }
        }

        private static int? _BigBreakLength;
        public static int BigBreakLength
        {
            get
            {
                if (_BigBreakLength == null)
                    _BigBreakLength = int.Parse(ConfigurationManager.AppSettings["big_break_length"]);

                return _BigBreakLength.Value;
            }
        }
    }
}