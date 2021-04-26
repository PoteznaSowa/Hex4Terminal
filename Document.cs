using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hex4Terminal {
	class Document {
		FileStream file;
		public string Name {
			get; private set;
		}

		public Document() {
			// Створити новий файл.
			file = null;
		}
		public Document(string path) {
			// Відкрити файл на комп'ютері.
			file = File.OpenRead(path);
		}

		~Document() {
			if(file != null) {

			}
		}
	}
}
