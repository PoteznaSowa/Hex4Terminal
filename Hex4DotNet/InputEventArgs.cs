using System;

namespace Hex4Terminal {
	class InputEventArgs: EventArgs {
		public ConsoleKeyInfo Key {
			get;
		}

		public InputEventArgs(ConsoleKeyInfo key) {
			Key = key;
		}
	}
}
