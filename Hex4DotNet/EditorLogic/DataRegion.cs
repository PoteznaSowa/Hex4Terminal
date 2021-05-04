using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hex4Terminal {
	abstract class DataRegion: Region {
		protected byte[] data;

		public byte this[int index] {
			get => data[index];
		}
	}
}
