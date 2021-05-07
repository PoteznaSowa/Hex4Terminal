namespace Hex4Terminal {
	abstract class UndoableChange {
		public long SizeDelta {
			get; protected set;
		}

		public long Location {
			get; protected set;
		}
	}
}
