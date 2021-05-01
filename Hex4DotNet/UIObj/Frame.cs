using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hex4Terminal {
	class Frame: IDrawable {
		public int X {
			get; set;
		}
		public int Y {
			get; set;
		}
		public int Width {
			get; set;
		}
		public int Height {
			get; set;
		}
		public ConsoleColor BGColour {
			get; set;
		}
		public ConsoleColor FGColour {
			get; set;
		}

		public Frame(int x, int y, int width, int height, ConsoleColor bgcolour, ConsoleColor fgcolour) {
			X = x;
			Y = y;
			Width = width;
			Height = height;
			BGColour = bgcolour;
			FGColour = fgcolour;
		}

		public void Draw() {
			Console.SetCursorPosition(X, Y);
			Console.BackgroundColor = BGColour;
			Console.ForegroundColor = FGColour;
			Console.Write($"┌{new string('─', Width - 2)}┐");
			for(int i = 1; i < Height - 1; i++) {
				Console.SetCursorPosition(X, Y + i);
				Console.Write('│');
				Console.SetCursorPosition(X + Width - 1, Y + i);
				Console.Write('│');
			}
			Console.Write($"└{new string('─', Width - 2)}┘");
		}
	}
}
