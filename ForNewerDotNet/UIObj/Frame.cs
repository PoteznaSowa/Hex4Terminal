using System;

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

		}
	}
}
