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
