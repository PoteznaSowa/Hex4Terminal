using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hex4Terminal {
	static class UI {
		public static object ConsoleUse = new();

		static Document doc = null;

		public static void Initialize() {

			InitInternal();
		}

		public static void Initialize(string filepath) {
			doc = new Document(filepath);
			InitInternal();
			ShowBytes();
		}

		static void InitInternal() {
			Program.KeyPress += ProcessInput;
			Program.ClockTick += UpdateClock;
			Program.WindowSizeChanged += RedrawScreen;
		}

		static void ShowBytes() {
			if(doc == null) {
				return;
			}

			FileStream file = doc.Stream;
			lock(ConsoleUse) {
				file.Position = position;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
				byte[] data = new byte[16];
				for(int i = 0; i < Console.WindowHeight - 3; i++) {
					Console.SetCursorPosition(0, 2 + i);
					int bytesread = file.Read(data);
					if(bytesread == 0) {
						break;
					}
					Console.Write($"{position + (i << 4):X8}");
					for(int j = 0; j < bytesread; j++) {
						Console.Write($" {data[j]:X2}");
					}
					Console.Write(new string(' ', (16 - bytesread) * 3));
					Console.Write(' ');
					for(int j = 0; j < bytesread; j++) {
						char c = (char)data[j];
						if(char.IsControl(c)) {
							Console.Write('.');
						} else {
							Console.Write((char)data[j]);
						}
					}
				}
			}
		}
		static void ShowLowerBytes() {
			if(doc == null) {
				return;
			}

			FileStream file = doc.Stream;
			int i = Console.WindowHeight - 4;
			file.Position = position + (i << 4);
			byte[] data = new byte[16];
			lock(ConsoleUse) {
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition(0, 2 + i);
				int bytesread = file.Read(data);
				if(bytesread == 0) {
					return;
				}
				Console.Write($"{position + (i << 4):X8}");
				for(int j = 0; j < bytesread; j++) {
					Console.Write($" {data[j]:X2}");
				}
				Console.Write(new string(' ', (16 - bytesread) * 3));
				Console.Write(' ');
				for(int j = 0; j < bytesread; j++) {
					char c = (char)data[j];
					if(char.IsControl(c)) {
						Console.Write('.');
					} else {
						Console.Write((char)data[j]);
					}
				}
			}
		}
		static void ShowUpperBytes() {
			if(doc == null) {
				return;
			}

			FileStream file = doc.Stream;
			lock(ConsoleUse) {
				file.Position = position;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
				byte[] data = new byte[16];
				int i = 0;
				Console.SetCursorPosition(0, 2 + i);
				int bytesread = file.Read(data);
				if(bytesread == 0) {
					return;
				}
				Console.Write($"{position + (i << 4):X8}");
				for(int j = 0; j < bytesread; j++) {
					Console.Write($" {data[j]:X2}");
				}
				Console.Write(new string(' ', (16 - bytesread) * 3));
				Console.Write(' ');
				for(int j = 0; j < bytesread; j++) {
					char c = (char)data[j];
					if(char.IsControl(c)) {
						Console.Write('.');
					} else {
						Console.Write((char)data[j]);
					}
				}
			}
		}

		static void ProcessInput(InputEventArgs e) {
			ConsoleKeyInfo cki = e.Key;
			switch(cki.Key) {
			case ConsoleKey.Escape:
				Program.Working = false;
				break;
			case ConsoleKey.UpArrow:
				ScrollUp();
				break;
			case ConsoleKey.DownArrow:
				ScrollDown();
				break;
			case ConsoleKey.Home:
				ScrollToStart();
				break;
			case ConsoleKey.End:
				ScrollToEnd();
				break;
			}
		}

		static long position = 0;

		static void ScrollDown() {
			if(position + 16 < doc.Stream.Length) {
				position += 16;
				if(OperatingSystem.IsWindows()) {
					lock(ConsoleUse) {
						Console.MoveBufferArea(0, 3, 73, Console.WindowHeight - 4, 0, 2);
					}
				}
				ShowLowerBytes();
			}
		}
		static void ScrollUp() {
			if(position > 0) {
				position -= 16;
				if(OperatingSystem.IsWindows()) {
					lock(ConsoleUse) {
						Console.MoveBufferArea(0, 2, 73, Console.WindowHeight - 4, 0, 3);
					}
				}
				ShowUpperBytes();
			}
		}
		static void ScrollToStart() {
			BlankBytes();
			position = 0;
			ShowBytes();
		}
		static void ScrollToEnd() {
			BlankBytes();
			position = doc.Stream.Length & -16;
			ShowBytes();
		}

		static void BlankBytes() {
			lock(ConsoleUse) {
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
				for(int i = 0; i < Console.WindowHeight - 3; i++) {
					Console.SetCursorPosition(0, 2 + i);
					Console.Write("                                                                         ");
				}
			}
		}
		static void BlankLowerBytes() {
			lock(ConsoleUse) {
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition(0, Console.WindowHeight - 2);
				Console.Write("                                                                         ");
			}
		}
		static void BlankUpperBytes() {
			lock(ConsoleUse) {
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition(0, 2);
				Console.Write("                                                                         ");
			}
		}

		static void UpdateClock() {
			lock(ConsoleUse) {
				Console.SetCursorPosition(Console.WindowWidth - 8, 0);
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Yellow;
				DateTime time = DateTime.Now;
				if(time.Millisecond < 500) {
					Console.Write($"{time:HH:mm:ss}");
				} else {
					Console.Write($"{time:HH mm ss}");
				}
				Console.ResetColor();
			}
		}

		public static void RedrawScreen() {

		}
	}
}
