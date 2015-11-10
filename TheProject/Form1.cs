﻿using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace TheProject
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			//Logger.Clear();

			Config.Initialize();

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

			DateTime now = DateTime.Now;
			TimeSpan lastInput = Win32.GetLastInputTime();

			if (lastInput.TotalSeconds > Config.Instance.BigBreakLengthInSeconds)
			{
				_LastMiniBreak = now;
				_LastBigBreak = _LastMiniBreak;
				IconState = new IconState();

				return;
			}

			if (lastInput.TotalSeconds > Config.Instance.MiniBreakLengthInSeconds)
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
				MiniBreakProgress = (int)(miniBreakElapsedSeconds / ((double)Config.Instance.MiniBreakIntervalInMinutes * 60 / ICON_DIMENSIONS)),
				BigBreakProgress = (int)(bigBreakElapsedSeconds / ((double)Config.Instance.BigBreakIntervalInMinutes * 60 / ICON_DIMENSIONS)),
				Paused = _Paused
			};

			if (!wasVisible &&
				miniBreakElapsedSeconds > Config.Instance.MiniBreakIntervalInMinutes * 60 &&
				lastInput.TotalSeconds < 2)
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
							color = Color.Red;

						int x = ICON_DIMENSIONS / 3;
						bitmap.SetPixel(x, y, color);
						if (y == 0)
							bitmap.SetPixel(x + 1, y, color);

						color = Color.Transparent;
						if (ICON_DIMENSIONS - y - 1 < _IconState.BigBreakProgress)
							color = Color.Blue;

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
			if (e.Button == MouseButtons.Right)
				Close();
			else if (e.Button == MouseButtons.Left)
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
	}
}