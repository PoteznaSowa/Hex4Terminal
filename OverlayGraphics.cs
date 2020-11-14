using System;
using System.Collections.Generic;
using System.Text;

namespace Hex4Terminal {
	abstract class OverlayGraphics {
		protected ConsoleColor FGColor {
			get; set;
		}
		protected ConsoleColor BGColor {
			get; set;
		}
		protected abstract void Draw();
	}
}
