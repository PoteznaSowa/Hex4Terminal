namespace Hex4Terminal {
	class RegionOverwritten: DataRegion {
		public RegionOverwritten(byte[] data, long position) {
			this.data = data;
			Size = data.Length;
			SizeDelta = 0;
			Position = position;
		}
	}
}
