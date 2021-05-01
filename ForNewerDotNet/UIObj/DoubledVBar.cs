using System;

namespace Hex4Terminal {
	class DoubledVBar: VerticalBar, IDrawable {
		public DoubledVBar(int height, ConsoleColor bgcolour, ConsoleColor fgcolour, int x, int y)
			: base(height, bgcolour, fgcolour, x, y) {
		}

		public override void Draw() {
			lock(UI.ConsoleUse) {
				Draw('║');
			}
		}
	}
}
