using System;
using System.Text;
using System.IO;

namespace Hex4Terminal {
	class Program {
		public static byte[] FileData;   // Безпосередньо сам файл.
		public static string FilePath;   // Шлях до файлу.
		public static bool FileChanged;  // Флаг зміни файлу.
		public static byte[] ClipBoard;  // Буфер обміну.
		public static bool InsertMode;   // Режим вставки.

		static int Main(string[] args) {
			Console.OutputEncoding = Encoding.UTF8;
			Console.InputEncoding = Encoding.Unicode;
			Console.Title = "Hex4Terminal";

			Console.Clear();
			for (int i = 0; i < 23; i++)
				Console.WriteLine("eeeeeeeee eeeeeeeee eeeeeeeee eeeeeeeee eeeeeeeee eeeeeeeee eeeeeeeee eeeeeeeee ");
			Console.MoveBufferArea(10, 10, 10, 10, 9, 9);
			Console.ReadKey();
			return 0;
			try {
				FileData = File.ReadAllBytes(args[0]);
				FilePath = Path.GetFullPath(args[0]);
			} catch {
				FileData = new byte[0];
				FilePath = "Untitled";
			}
			Screen.Initialize();

			return 0;
		}
	}
}
