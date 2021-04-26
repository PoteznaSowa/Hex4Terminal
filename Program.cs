using System;
using System.Text;
using System.Threading;

namespace Hex4Terminal {
	static class Program {
		static void Main(string[] args) {
			Console.OutputEncoding = Encoding.UTF8;
			Console.InputEncoding = Encoding.Unicode;
			Console.Title = "Hex4Terminal";

			Console.CursorVisible = false;
			Console.BackgroundColor = ConsoleColor.Black;
			Console.Clear();
			Thread clock = new(ClockLoop);
			clock.Start();
			while(MainLoop()) { }

			clock.Join();
		}

		static bool MainLoop() {



			if(Console.KeyAvailable) {
				KeyPress(new InputEventArgs(Console.ReadKey(true)));
			} else {
				Thread.Sleep(1);
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
			do {
				int ms = DateTime.Now.Millisecond;
				if(ms < 500) {
					Thread.Sleep(500 - ms);
				} else {
					Thread.Sleep(1000 - ms);
				}
				ClockTick();
			} while(Working);
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
