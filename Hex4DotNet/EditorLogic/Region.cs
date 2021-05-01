using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hex4Terminal {
	abstract class Region {
		public long Size {
			get; protected set;
		}
		public int this[long position] {
			get => ReadByte(position);
		}
		protected abstract int ReadByte(long position);
	}
}
