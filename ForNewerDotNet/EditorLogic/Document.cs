using System.IO;

namespace Hex4Terminal {
	class Document {
		public FileStream Stream {
			get; private set;
		}

		public string Name {
			get; private set;
		}

		public Document() {
			Stream = null;
		}
		public Document(string path) {
			Stream = File.OpenRead(path);
		}

		~Document() {
			if(Stream != null) {
				Stream.Dispose();
			}
		}
	}
}
