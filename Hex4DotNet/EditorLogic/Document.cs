using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hex4Terminal {
	class Document {
		public FileStream Stream {
			get; private set;
		}

		public string Name {
			get; private set;
		}

		public long Size {
			get {
				return Stream.Length;
			}
		}

		List<Region> regions = new List<Region>(8);
		List<UndoableChange> undobuffer = new List<UndoableChange>(8);

		public int ReadByteAt(long position) {
			return -1;
		}

		public Document() {
			// Створити новий файл.
			Stream = null;
		}
		public Document(string path) {
			// Відкрити файл на комп'ютері.
			Stream = File.OpenRead(path);
		}

		~Document() {
			if(Stream != null) {
				Stream.Dispose();
			}
		}
	}
}
