using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiamondDash
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
			this.Opacity = .7;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			mydisplay = Screen.PrimaryScreen;
			screenshot = new Bitmap(mydisplay.Bounds.Width, mydisplay.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
		}

		Bitmap screenshot;
		Screen mydisplay;
		int[,] board = new int[15, 15];
		void ScanScreen()
		{
			var gfx = Graphics.FromImage(screenshot);
			gfx.CopyFromScreen(mydisplay.Bounds.X, mydisplay.Bounds.Y, 0, 0, mydisplay.Bounds.Size, CopyPixelOperation.SourceCopy);
			

			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					var col = screenshot.GetPixel(targetX + i * 40, targetY + j * 40);
					board[i, j] = CodeFromColor(col);
				}
			}

		}
		private int CodeFromColor(Color color)
		{
			if (color.B < 10)
			{
				if (color.R > 200)
					if (color.G > 150)
					{
						return 1;//Yellow
					}
			}
			if (color.R > 200)
			{
				if ((color.G < 20) && (color.B < 20))
					return 2;//Red;
			}

			if (color.R < 20)
			{
				if (color.G > 90 && color.G < 120)
				{
					if (color.B > 240)
						return 3;//blue;
				}
			}
			if (color.G > 170)
			{
				if (color.R < 20 && color.B < 20)
				{
					return 4;//green;
				}
			}

			if (color.R > 170 && color.R < 190)
			{
				if (color.G > 70 && color.G < 150)
				{
					if (color.B > 200)
					{
						return 5;//Magenda;
					}
				}
			}

			return 0;
		}

		int targetX, targetY;
		private void Form1_MouseClick(object sender, MouseEventArgs e)
		{
			targetX = e.X;
			targetY = e.Y;
			this.WindowState = FormWindowState.Minimized;
			
			Do();
			
		}


		private void FindTarget()
		{			
			for (int j = 0; j < 9; j++)
				for (int i = 0; i < 10; i++)
					for (int code = 1; code <= 5; code++)
						if (Find(code, i, j, new int[15, 15]) >= 3)
						{
							Win32.SetCursorPos(targetX + i * 40, targetY + j * 40);
							System.Threading.Thread.Sleep(100);
							MouseSimulator.ClickLeftMouseButton();
							System.Threading.Thread.Sleep(50);							
						}

			
		}

		
		private int Find(int code, int x, int y, int[,] path)
		{
			if ((x > -1 && x < 10) && (y > -1 && y < 9))
			{
				if (board[x, y] == code)
				{
					path[x, y] = 1;
					board[x, y] = 0;
					int right = 0, bottom = 0, left = 0, top = 0;
					if (path[x + 1, y] != 1)
						right = Find(code, x + 1, y, path);

					if (path[x, y + 1] != 1)
						bottom = Find(code, x, y + 1, path);

					if (x > 0)
						if (path[x - 1, y] != 1)
							left = Find(code, x - 1, y, path);

					if (y > 0)
						if (path[x, y - 1] != 1)
							top = Find(code, x, y - 1, path);
					return right + bottom + left + 1;
				}
				else return 0;
			}
			else return 0;
		}
		void Do()
		{
			for (int i = 0; i < 570; i++)
			{
				ScanScreen();				
				FindTarget();				
			}
			
		}

		//BackgroundWorker bg = new BackgroundWorker();

	}

	public enum MyPos
	{
		Left, Right, Bottom, Top, None
	}

	public class Win32
	{
		[DllImport("User32.Dll")]
		public static extern long SetCursorPos(int x, int y);

		[DllImport("User32.Dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

		[StructLayout(LayoutKind.Sequential)]
		public struct POINT
		{
			public int x;
			public int y;
		}

	}

	public class MouseSimulator
	{
		[DllImport("user32.dll", SetLastError = true)]
		static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

		[StructLayout(LayoutKind.Sequential)]
		struct INPUT
		{
			public SendInputEventType type;
			public MouseKeybdhardwareInputUnion mkhi;
		}
		[StructLayout(LayoutKind.Explicit)]
		struct MouseKeybdhardwareInputUnion
		{
			[FieldOffset(0)]
			public MouseInputData mi;

			[FieldOffset(0)]
			public KEYBDINPUT ki;

			[FieldOffset(0)]
			public HARDWAREINPUT hi;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct KEYBDINPUT
		{
			public ushort wVk;
			public ushort wScan;
			public uint dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}
		[StructLayout(LayoutKind.Sequential)]
		struct HARDWAREINPUT
		{
			public int uMsg;
			public short wParamL;
			public short wParamH;
		}
		struct MouseInputData
		{
			public int dx;
			public int dy;
			public uint mouseData;
			public MouseEventFlags dwFlags;
			public uint time;
			public IntPtr dwExtraInfo;
		}
		[Flags]
		enum MouseEventFlags : uint
		{
			MOUSEEVENTF_MOVE = 0x0001,
			MOUSEEVENTF_LEFTDOWN = 0x0002,
			MOUSEEVENTF_LEFTUP = 0x0004,
			MOUSEEVENTF_RIGHTDOWN = 0x0008,
			MOUSEEVENTF_RIGHTUP = 0x0010,
			MOUSEEVENTF_MIDDLEDOWN = 0x0020,
			MOUSEEVENTF_MIDDLEUP = 0x0040,
			MOUSEEVENTF_XDOWN = 0x0080,
			MOUSEEVENTF_XUP = 0x0100,
			MOUSEEVENTF_WHEEL = 0x0800,
			MOUSEEVENTF_VIRTUALDESK = 0x4000,
			MOUSEEVENTF_ABSOLUTE = 0x8000
		}
		enum SendInputEventType : int
		{
			InputMouse,
			InputKeyboard,
			InputHardware
		}

		public static void ClickLeftMouseButton()
		{
			INPUT mouseDownInput = new INPUT();
			mouseDownInput.type = SendInputEventType.InputMouse;
			mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTDOWN;
			SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

			INPUT mouseUpInput = new INPUT();
			mouseUpInput.type = SendInputEventType.InputMouse;
			mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_LEFTUP;
			SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
		}
		public static void ClickRightMouseButton()
		{
			INPUT mouseDownInput = new INPUT();
			mouseDownInput.type = SendInputEventType.InputMouse;
			mouseDownInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTDOWN;
			SendInput(1, ref mouseDownInput, Marshal.SizeOf(new INPUT()));

			INPUT mouseUpInput = new INPUT();
			mouseUpInput.type = SendInputEventType.InputMouse;
			mouseUpInput.mkhi.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_RIGHTUP;
			SendInput(1, ref mouseUpInput, Marshal.SizeOf(new INPUT()));
		}
	}
}
