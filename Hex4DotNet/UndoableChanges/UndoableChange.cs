using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hex4Terminal {
	abstract class UndoableChange {
		public long SizeDelta {
			get; protected set;
		}

		public long Location {
			get; protected set;
		}
	}
}
