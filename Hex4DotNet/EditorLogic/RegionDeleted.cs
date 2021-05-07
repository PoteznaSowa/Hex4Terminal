namespace Hex4Terminal {
	class RegionDeleted: Region {
		public RegionDeleted(long size, long position) {
			Size = size;
			SizeDelta = -size;
			Position = position;
		}
	}
}
