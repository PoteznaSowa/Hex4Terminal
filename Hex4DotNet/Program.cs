﻿using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace Hex4Terminal {
	static class Program {
		[STAThread]
		static void Main(string[] args) {
			Console.OutputEncoding = Encoding.UTF8;
			Console.InputEncoding = Encoding.Unicode;
			Console.Title = "Hex4Terminal";

			Console.CursorVisible = false;
			Console.TreatControlCAsInput = true;

			Console.WindowWidth = 120;
			Console.BufferWidth = 120;
			Console.WindowHeight = 30;
			wHeight = 30;
			wWidth = 120;
			if(Console.BufferHeight < 50) {
				Console.BufferHeight = 999;
			}
			InHandle = GetStdHandle(-10);

			Console.BackgroundColor = ConsoleColor.Black;
			//Console.BackgroundColor = ConsoleColor.White;
			Console.Clear();

			Thread clock = new Thread(ClockLoop);
			try {
				if(args.Length > 0) {
					UI.Initialize(args[0]);
				} else {
					UI.Initialize();
				}
				clock.Priority = ThreadPriority.Lowest;
				clock.Start();

				while(MainLoop()) { }
			} finally {

				if(clock.IsAlive) {
					clock.Priority = ThreadPriority.Highest;
					clock.Interrupt();
					clock.Join();
				}

				Console.ResetColor();
				Console.Clear();
				Console.CursorVisible = true;
				Console.TreatControlCAsInput = false;
			}
		}

		static int wWidth;
		static int wHeight;

		static IntPtr InHandle = GetStdHandle(-10);

		[DllImport("KERNEL32.DLL")]
		static extern IntPtr GetStdHandle(int nStdHandle);

		[DllImport("KERNEL32.DLL")]
		static extern uint WaitForSingleObjectEx(
			IntPtr hHandle, uint dwMilliseconds, bool bAlertable
			);

		static bool MainLoop() {
			if(Console.WindowWidth != wWidth || Console.WindowHeight != wHeight) {
				Console.CursorVisible = false;
				WindowSizeChanged();
			}

			const uint timeout = 60 * 1000;

			// Перевірити наявність нового користувацького вводу.
			if(WaitForSingleObjectEx(InHandle, timeout, true) == 0
				&& Console.KeyAvailable)
			{
				KeyPress(new InputEventArgs(Console.ReadKey(true)));
				ClearInputBuffer();
			}

			return Working;
		}

		static void ClearInputBuffer() {
			while(Console.KeyAvailable) {
				Console.ReadKey(true);
			}
		}

		static void ClockLoop() {
			try {
				do {
					int ms = DateTime.Now.Millisecond;
					if(ms < 500) {
						Thread.Sleep(500 - ms);
					} else {
						Thread.Sleep(1000 - ms);
					}
					ClockTick();
				} while(Working);
			} catch { }
		}

		public static volatile bool Working = true;

		public delegate void InputEventHandler(InputEventArgs e);

		public static event InputEventHandler KeyPress = Dummy;
		public static event Action WindowSizeChanged = Dummy;
		public static event Action ClockTick = Dummy;

		static void Dummy() { }
		static void Dummy(object o) { }
	}
}
