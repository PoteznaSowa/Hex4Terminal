using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace Hex4Terminal {
	static class Program {
		static void Main(string[] args) {
			Console.OutputEncoding = Encoding.UTF8;
			Console.InputEncoding = Encoding.Unicode;
			Console.Title = "Hex4Terminal";

			Console.CursorVisible = false;
			Console.TreatControlCAsInput = true;

			wWidth = Console.WindowWidth;
			wHeight = Console.WindowHeight;
			if(OperatingSystem.IsWindows()) {
				wWidth = 120;
				Console.BufferWidth = 120;
				Console.WindowWidth = 120;
				if(Console.BufferHeight < 50) {
					Console.BufferHeight = 999;
				}
				OutHandle = GetStdHandle(-10);
			}

			Console.BackgroundColor = ConsoleColor.Black;
			Console.Clear();

			if(args.Length > 0) {
				UI.Initialize(args[0]);
			} else {
				UI.Initialize();
			}

			Thread clock = new(ClockLoop);
			clock.Start();

			while(MainLoop()) { }

			clock.Interrupt();
			clock.Join();

			Console.ResetColor();
			Console.Clear();
			Console.CursorVisible = true;
			Console.TreatControlCAsInput = false;
		}

		static int wWidth;
		static int wHeight;


		// Дані та функції для застосування в Windows.
		static IntPtr OutHandle;  // Вказівник на дескриптор stdin.

		// Отримати вказівник на дескриптор stdout/stdin/stderr.
		[DllImport("KERNEL32.DLL")]
		static extern IntPtr GetStdHandle(int nStdHandle);
		// Чекати на зміну стану об'єкту впродовж певного часу.
		[DllImport("KERNEL32.DLL")]
		static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);


		static bool MainLoop() {
			bool inwindows = OperatingSystem.IsWindows();

			// Перевірити, якщо змінився розмір вікна.
			if(Console.WindowWidth != wWidth || Console.WindowHeight != wHeight) {
				lock(UI.ConsoleUse) {
					Console.Clear();
					wWidth = Console.WindowWidth;
					if(inwindows) {
						Console.BufferWidth = Console.WindowWidth;
					}
					wHeight = Console.WindowHeight;
				}
				WindowSizeChanged();
			}

			// Перевірити наявність нового користувацького вводу.
			if(inwindows) {
				if(WaitForSingleObject(OutHandle, 8) == 0 && Console.KeyAvailable) {
					KeyPress(new InputEventArgs(Console.ReadKey(true)));
				}
			} else if(Console.KeyAvailable) {
				KeyPress(new InputEventArgs(Console.ReadKey(true)));
			} else {
				Thread.Sleep(8);
			}

			return Working;
		}
		/*
		static void InputLoop() {
			do {
				if(Console.KeyAvailable) {
					keyqueue.Enqueue(Console.ReadKey(true));
				} else {
					Thread.Sleep(1);
				}
			} while(Working);
		}
		*/
		static void ClockLoop() {
			// Викликати подію кожні півсекунди.
			// У випадку будь-якого винятка припинити роботу.
			try {
				for(; ; ) {
					int ms = DateTime.Now.Millisecond;
					if(ms < 500) {
						Thread.Sleep(500 - ms);
					} else {
						Thread.Sleep(1000 - ms);
					}
					ClockTick();
				}
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
