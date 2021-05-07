using System;

namespace Hex4Terminal {
	class GoToBox: OptionBox {
		public override void UserInput(ConsoleKeyInfo cki) {
			switch(cki.Key) {
			case ConsoleKey.Enter:
				UI.GoTo(num);
				goto case ConsoleKey.Escape;
			case ConsoleKey.Escape:
				UI.optionBox = null;
				ClearBox();
				break;
			case ConsoleKey.D0:
			case ConsoleKey.D1:
			case ConsoleKey.D2:
			case ConsoleKey.D3:
			case ConsoleKey.D4:
			case ConsoleKey.D5:
			case ConsoleKey.D6:
			case ConsoleKey.D7:
			case ConsoleKey.D8:
			case ConsoleKey.D9:
				num = ((num << 4) & 0xF_FFFF_FFFF)
					+ (cki.Key - ConsoleKey.D0);
				UpdateDisplay();
				break;
			case ConsoleKey.A:
			case ConsoleKey.B:
			case ConsoleKey.C:
			case ConsoleKey.D:
			case ConsoleKey.E:
			case ConsoleKey.F:
				num = ((num << 4) & 0xF_FFFF_FFFF)
					+ (cki.Key - ConsoleKey.A + 10);
				UpdateDisplay();
				break;
			case ConsoleKey.NumPad0:
			case ConsoleKey.NumPad1:
			case ConsoleKey.NumPad2:
			case ConsoleKey.NumPad3:
			case ConsoleKey.NumPad4:
			case ConsoleKey.NumPad5:
			case ConsoleKey.NumPad6:
			case ConsoleKey.NumPad7:
			case ConsoleKey.NumPad8:
			case ConsoleKey.NumPad9:
				num = ((num << 4) & 0xF_FFFF_FFFF)
					+ (cki.Key - ConsoleKey.NumPad0);
				UpdateDisplay();
				break;
			}
		}

		static long num = 0;

		public GoToBox() {
			lock(UI.ConsoleUse) {
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				Console.SetCursorPosition(UI.RowSize, 3);
				Console.Write("Введiть адресу переходу");
			}
			UpdateDisplay();
		}

		void ClearBox() {
			lock(UI.ConsoleUse) {
				Console.BackgroundColor = ConsoleColor.Black;
				Console.SetCursorPosition(UI.RowSize, 3);
				Console.Write("                       ");
				Console.SetCursorPosition(UI.RowSize, 4);
				Console.Write("          ");
			}
		}

		void UpdateDisplay() {
			lock(UI.ConsoleUse) {
				Console.BackgroundColor = ConsoleColor.DarkBlue;
				Console.ForegroundColor = ConsoleColor.DarkYellow;
				Console.SetCursorPosition(UI.RowSize, 4);
				Console.Write($"{num:X9}h");
			}
		}
	}
}
