using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hex4Terminal {
	class FileRef: Region {
		FileStream file;
		long position;

		public FileRef(FileStream file, long position, long size) {
			this.file = file;
			this.position = position;
			Size = size;
		}

		protected override int ReadByte(long position) {
			long p = this.position + position;
			if(p < file.Length) {
				file.Position = p;
				return file.ReadByte();
			} else {
				return -1;
			}
		}
	}
}
