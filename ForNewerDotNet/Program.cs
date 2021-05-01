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
				Console.WindowWidth = 120;
				Console.BufferWidth = 120;
				if(Console.BufferHeight < 50) {
					Console.BufferHeight = 999;
				}
				InHandle = GetStdHandle(-10);
			}

			Console.BackgroundColor = ConsoleColor.Black;
			Console.Clear();

			Thread clock = new(ClockLoop);
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
				// Якщо щось станеться, все одно завершити програму коректно.

				clock.Priority = ThreadPriority.Highest;
				clock.Interrupt();  
				clock.Join();

				Console.ResetColor();
				Console.Clear();
				Console.CursorVisible = true;
				Console.TreatControlCAsInput = false;
			}
		}

		static int wWidth;
		static int wHeight;

		// Дані та функції для застосування в Windows.
		static IntPtr InHandle;  // Вказівник на дескриптор stdin.

		// Отримати вказівник на дескриптор stdout/stdin/stderr.
		[DllImport("KERNEL32.DLL")]
		static extern IntPtr GetStdHandle(int nStdHandle);
		// Чекати на зміну стану об'єкту.
		[DllImport("KERNEL32.DLL")]
		static extern uint WaitForSingleObjectEx(
			IntPtr hHandle, uint dwMilliseconds, bool bAlertable
			);

		static bool MainLoop() {
			// Перевірити, якщо змінився розмір вікна.
			if(Console.WindowWidth != wWidth || Console.WindowHeight != wHeight) {
				wWidth = Console.WindowWidth;
				wHeight = Console.WindowHeight;
				WindowSizeChanged();  // Виклик події.
			}

			// Перевірити наявність нового користувацького вводу.
			if(OperatingSystem.IsWindows()) {
				// У Windows чекати на повідомлення, що надходять до вікна консолі.
				if(WaitForSingleObjectEx(InHandle, uint.MaxValue, true) == 0 && Console.KeyAvailable) {
					KeyPress(new InputEventArgs(Console.ReadKey(true)));
					ClearInputBuffer();
				}
			} else if(Console.KeyAvailable) {
				KeyPress(new InputEventArgs(Console.ReadKey(true)));
				ClearInputBuffer();
			} else {
				Thread.Sleep(8);
			}

			return Working;
		}

		static void ClearInputBuffer() {
			while(Console.KeyAvailable) {
				Console.ReadKey(true);
			}
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
