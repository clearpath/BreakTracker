
namespace TheProject
{
    internal class IconState
    {
        public int MiniBreakProgress { get; set; }
        public int BigBreakProgress { get; set; }
		public bool Paused { get; set; }

        public override bool Equals(object obj)
        {
            IconState other = obj as IconState;
            if (other == null)
                return false;

            return 
				MiniBreakProgress == other.MiniBreakProgress &&
                BigBreakProgress == other.BigBreakProgress &&
				Paused == other.Paused;
        }

        public override int GetHashCode()
        {
            return MiniBreakProgress;
        }
    }
}