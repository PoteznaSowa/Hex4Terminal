namespace Hex4Terminal {
	abstract class Region {
		public long Size {
			get; protected set;
		}
		public long SizeDelta {
			get; protected set;
		}
		public long Position {
			get; protected set;
		}
	}
}
