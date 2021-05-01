using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hex4Terminal {
	class RegionDeleted: UndoableChange {
		public RegionDeleted(long size, long location) {
			Location = location;
			SizeDelta = -size;
		}
	}
}
