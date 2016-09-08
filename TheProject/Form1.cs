using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace TheProject
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			InitContextMenu();
			
			//Logger.Clear();

			
			IconState = new IconState();
			Visible = false;
			TopMost = true;
		}

		private DateTime _LastMiniBreak = DateTime.Now;
		private DateTime _LastBigBreak = DateTime.Now;

		private const int ICON_DIMENSIONS = 16;

		private void _Heartbeat_Tick(object sender, EventArgs e)
		{
			bool wasVisible = Visible;
			if (Visible)
				Visible = false;

			var config = Config.Instance;

			DateTime now = DateTime.Now;
			TimeSpan timeSpanSinceLastInput = Win32.GetTimeSpanSinceLastInput();
			
			if (timeSpanSinceLastInput.TotalSeconds > config.BigBreakLengthInSeconds)
			{
				_LastMiniBreak = now;
				_LastBigBreak = now;
				IconState = new IconState();

				return;
			}

			if (timeSpanSinceLastInput.TotalSeconds > config.MiniBreakLengthInSeconds)
			{
				_LastMiniBreak = now;
				IconState = new IconState { BigBreakProgress = IconState.BigBreakProgress };
				ResetNotifier();

				return;
			}

			double miniBreakElapsedSeconds = (now - _LastMiniBreak).TotalSeconds;
			double bigBreakElapsedSeconds = (now - _LastBigBreak).TotalSeconds;
			//Logger.Log("ElapsedSeconds: {0}", elapsedSeconds);

			IconState = new IconState
			{
				MiniBreakProgress = (int)(miniBreakElapsedSeconds / ((double)config.MiniBreakIntervalInMinutes * 60 / ICON_DIMENSIONS)),
				BigBreakProgress = (int)(bigBreakElapsedSeconds / ((double)config.BigBreakIntervalInMinutes * 60 / ICON_DIMENSIONS)),
				Paused = _Paused
			};

			if (!wasVisible &&
				miniBreakElapsedSeconds > config.MiniBreakIntervalInMinutes * 60 &&
				timeSpanSinceLastInput.TotalSeconds < 2)
				Notify();
		}

		void ResetNotifier()
		{
			_Multiplier = 1;
		}


		private IconState _IconState;
		private IconState IconState
		{
			get { return _IconState; }
			set
			{
				if (_IconState == null ||
					!_IconState.Equals(value))
				{
					_IconState = value;
					Bitmap bitmap = new Bitmap(ICON_DIMENSIONS, ICON_DIMENSIONS);
					for (int y = 0; y < ICON_DIMENSIONS; y++)
					{
						Color color = Color.Transparent;
						if (ICON_DIMENSIONS - y - 1 < _IconState.MiniBreakProgress)
							color = Color.Tomato;

						int x = ICON_DIMENSIONS * 1 / 3;
						bitmap.SetPixel(x, y, color);
						if (y == 0)
							bitmap.SetPixel(x + 1, y, color);

						color = Color.Transparent;
						if (ICON_DIMENSIONS - y - 1 < _IconState.BigBreakProgress)
							color = Color.LightSkyBlue;

						x = ICON_DIMENSIONS * 2 / 3;
						bitmap.SetPixel(x, y, color);
						if (y == 0)
							bitmap.SetPixel(x + 1, y, color);
					}

					if (_IconState.Paused)
						DrawP(bitmap);

					Icon icon = Icon.FromHandle(bitmap.GetHicon());
					_TrayIcon.Icon = icon;
					Win32.DestroyIcon(icon.Handle);

					// Logger.Log("Icon progress: {0}", _IconProgress);                                        
				}
			}
		}

		static void DrawP(Bitmap bitmap)
		{
			Color color = Color.Black;
			int x = 0;
			int y = ICON_DIMENSIONS - 1;
			bitmap.SetPixel(x, y, color);

			y--;
			bitmap.SetPixel(x, y, color);

			y--;
			bitmap.SetPixel(x, y, color);

			y--;
			bitmap.SetPixel(x, y, color);

			x++;
			bitmap.SetPixel(x, y, color);

			x++;
			y--;
			bitmap.SetPixel(x, y, color);

			x--;
			y--;
			bitmap.SetPixel(x, y, color);

			x--;
			bitmap.SetPixel(x, y, color);

			y++;
			bitmap.SetPixel(x, y, color);
		}

		private double _Multiplier = 1;

		private void Notify()
		{
			if (!Config.Instance.NotifyOnBreak)
				return;

			if (_Paused)
				return;

			Rectangle rect = Screen.PrimaryScreen.WorkingArea;
			int height = 1;
			int width = (int)(200 * _Multiplier);
			Bounds = new Rectangle(0, rect.Bottom - height, width, height);

			Visible = true;
			_Multiplier *= 1.2;
		}

		private void _TrayIcon_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
				TogglePaused();
		}

		bool _Paused;

		private void TogglePaused()
		{
			_Paused = !_Paused;
			Hide();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			Hide();
		}

		private void InitContextMenu()
		{
			var contextMenu = new ContextMenuStrip();
			contextMenu.Items.Add("Pause", null, PauseClicked);
			contextMenu.Items.Add("Open config", null, OpenConfigClicked);
			contextMenu.Items.Add("Exit", null, ExitClicked);
			_TrayIcon.ContextMenuStrip = contextMenu;
		}

		private void OpenConfigClicked(object sender, EventArgs e)
		{
			Process.Start(Config.FILE_NAME);
		}

		private void ExitClicked(object sender, EventArgs e)
		{
			Close();
		}

		private void PauseClicked(object sender, EventArgs e)
		{
			TogglePaused();
		}
	}
}