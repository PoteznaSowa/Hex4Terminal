namespace Hex4Terminal {
	class RegionInserted: DataRegion {
		public RegionInserted(byte[] data, long position) {
			this.data = data;
			Size = data.Length;
			SizeDelta = Size;
			Position = position;
		}
	}
}
