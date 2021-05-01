using System;

namespace Hex4Terminal {
	class VerticalBar: IDrawable {
		public int Height {
			get; set;
		}

		public int X {
			get; set;
		}

		public int Y {
			get; set;
		}

		public ConsoleColor BGColour {
			get; set;
		}

		public ConsoleColor FGColour {
			get; set;
		}

		public VerticalBar(int height, ConsoleColor bgcolour, ConsoleColor fgcolour, int x, int y) {
			Height = height;
			BGColour = bgcolour;
			FGColour = fgcolour;
			X = x;
			Y = y;
		}

		public virtual void Draw() {
			lock(UI.ConsoleUse) {
				Draw('│');
			}
		}

		protected void Draw(char c) {
			Console.BackgroundColor = BGColour;
			Console.ForegroundColor = FGColour;
			Console.CursorLeft = X;
			for(int i = 0; i < Height; i++) {
				Console.CursorTop = Y + i;
				Console.Write(c);
			}
		}
	}
}
