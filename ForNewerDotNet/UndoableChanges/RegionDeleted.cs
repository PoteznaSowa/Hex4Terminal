namespace Hex4Terminal {
	class RegionDeleted: UndoableChange {
		public RegionDeleted(long size, long location) {
			Location = location;
			SizeDelta = -size;
		}
	}
}
