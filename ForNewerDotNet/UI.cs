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
		static List<UndoableChange> undobuffer = new(8);

		public static void Initialize() {


			InitInternal();
		}

		public static void Initialize(string filepath) {
			doc = new Document(filepath);
			Console.SetCursorPosition(0, 1);
			Console.WriteLine(Path.GetFileName(filepath));
			ShowBytes();
			InitInternal();
		}

		static void InitInternal() {
			Program.KeyPress += ProcessInput;
			Program.ClockTick += UpdateClock;
			Program.WindowSizeChanged += RedrawScreen;
		}

		static readonly StringBuilder _builder = new(73);
		static void ShowRowOfBytes(int offset) {
			if(doc == null) {
				return;
			}

			FileStream file = doc.Stream;
			file.Position = position + (offset << 4);
			byte[] data = new byte[16];
			int bytesread = file.Read(data);
			if(bytesread == 0) {
				return;
			}
			_builder.Clear();
			_builder.Append($"{position + (offset << 4):X8}");
			for(int j = 0; j < bytesread; j++) {
				_builder.Append($" {data[j]:X2}");
			}
			_builder.Append(new string(' ', (16 - bytesread) * 3 + 1));
			for(int j = 0; j < bytesread; j++) {
				char c = (char)data[j];
				if(char.IsControl(c)) {
					_builder.Append('.');
				} else {
					_builder.Append((char)data[j]);
				}
			}
			lock(ConsoleUse) {
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
				Console.SetCursorPosition(0, 2 + offset);
				Console.Write(_builder);
			}
		}
		static void ShowBytes() {
			for(int i = 0; i < Console.WindowHeight - 3; i++) {
				ShowRowOfBytes(i);
			}
		}
		static void ShowLowerBytes() {
			ShowRowOfBytes(Console.WindowHeight - 4);
		}
		static void ShowUpperBytes() {
			ShowRowOfBytes(0);
		}

		static void ProcessInput(InputEventArgs e) {
			ConsoleKeyInfo cki = e.Key;
			switch(cki.Key) {
			case ConsoleKey.Escape:
				Program.Working = false;
				break;
			case ConsoleKey.PageUp:
				PageUpScroll();
				break;
			case ConsoleKey.PageDown:
				PageDownScroll();
				break;
			case ConsoleKey.End:
				ScrollToEnd();
				break;
			case ConsoleKey.Home:
				ScrollToStart();
				break;
			case ConsoleKey.UpArrow:
				ScrollUp();
				break;
			case ConsoleKey.DownArrow:
				ScrollDown();
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
					ShowLowerBytes();
				} else {
					BlankBytes();
					ShowBytes();
				}
			}
		}
		static void ScrollUp() {
			if(position > 0) {
				position -= 16;
				if(OperatingSystem.IsWindows()) {
					lock(ConsoleUse) {
						Console.MoveBufferArea(0, 2, 73, Console.WindowHeight - 4, 0, 3);
					}
					ShowUpperBytes();
				} else {
					BlankBytes();
					ShowBytes();
				}
			}
		}
		static void ScrollToStart() {
			if(position != 0) {
				BlankBytes();
				position = 0;
				ShowBytes();
			}
		}
		static void ScrollToEnd() {
			if(position != (doc.Stream.Length & -16)) {
				BlankBytes();
				position = doc.Stream.Length & -16;
				ShowBytes();
			}
		}
		static void PageUpScroll() {
			if(position - ((Console.WindowHeight - 4) << 4) < 0) {
				ScrollToStart();
				return;
			}
			BlankBytes();
			position -= (Console.WindowHeight - 4) << 4;
			ShowBytes();
		}
		static void PageDownScroll() {
			if(position + ((Console.WindowHeight - 4) << 4) >= doc.Stream.Length) {
				ScrollToEnd();
				return;
			}
			BlankBytes();
			position += (Console.WindowHeight - 4) << 4;
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
			Console.Clear();
			ShowBytes();
		}
	}
}
