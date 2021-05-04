using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hex4Terminal {
	class RegionDeleted: Region {
		public RegionDeleted(long size, long position) {
			Size = size;
			SizeDelta = -size;
			Position = position;
		}
	}
}
