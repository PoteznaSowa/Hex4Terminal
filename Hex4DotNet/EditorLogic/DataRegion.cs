namespace Hex4Terminal {
	abstract class DataRegion: Region {
		protected byte[] data;

		public byte this[int index] {
			get => data[index];
		}
	}
}
