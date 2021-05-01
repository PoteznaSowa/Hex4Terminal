using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hex4Terminal {
	class ArbitraryData: Region {
		byte[] data;

		public ArbitraryData(byte[] data) {
			this.data = data;
		}

		protected override int ReadByte(long position) {
			return position < data.Length ? data[position] : -1;
		}
	}
}
