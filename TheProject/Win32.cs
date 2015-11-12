using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace TheProject
{
    internal static class Win32
    {
        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static TimeSpan GetTimeSpanSinceLastInput()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;
			
            if (GetLastInputInfo(ref lastInputInfo))
            {
                long lastInputTick = lastInputInfo.dwTime;
                long milliseconds = Environment.TickCount - lastInputTick;

                int seconds = (int)(milliseconds / 1000);
                TimeSpan result = new TimeSpan(0, 0, seconds);

                //Logger.Log("ResultTimeSpanSeconds: {0}", result.TotalSeconds);

                return result;
            }

            throw new Win32Exception();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public extern static bool DestroyIcon(IntPtr handle);
    }
}