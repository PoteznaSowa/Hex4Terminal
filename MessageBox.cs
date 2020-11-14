using System;
using System.Collections.Generic;
using System.Text;

namespace Hex4Terminal {
	class MessageBox : OverlayGraphics, IDialog {
		// MessageBox — просте діалогове вікно, у якому треба натиснути Yes/No, OK/Cancel або т.д.
		protected override void Draw() {
			
		}
		public DialogResult Show() {
			return 0;
		}
	}
}
