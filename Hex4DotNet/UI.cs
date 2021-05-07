using System;
using System.Text;
using System.IO;
using Microsoft.Win32;

namespace Hex4Terminal {
	static class UI {
		public static object ConsoleUse = new object();

		static Document doc = null;
		static readonly OpenFileDialog oFileDialog = new OpenFileDialog();
		static readonly SaveFileDialog sFileDialog = new SaveFileDialog();

		public static void Initialize() {
			InitInternal();
		}

		public static void Initialize(string filepath) {
			doc = new Document(filepath);
			string folder = Path.GetDirectoryName(Path.GetFullPath(filepath));
			oFileDialog.InitialDirectory = folder;
			sFileDialog.InitialDirectory = folder;
			sFileDialog.Filter = "Усі файли|*.*";
			Console.SetCursorPosition(0, 1);
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine(Path.GetFileName(filepath));
			ShowBytes();
			HighlightCursor();
			InitInternal();
		}

		static void InitInternal() {
			Program.KeyPress += ProcessInput;
			Program.ClockTick += UpdateClock;
			Program.WindowSizeChanged += RedrawScreen;
		}

		static readonly StringBuilder _builder = new StringBuilder(RowSize);
		public const int RowSize = 74; // 9 + 16 * 3 + 17
		static void ShowRowOfBytes(int offset) {
			if(doc == null) {
				return;
			}

			long pos = position & -16;
			byte[] data = doc.Read(pos + (offset << 4), 16);
			int bytesread = data.Length;
			if(bytesread == 0) {
				return;
			}
			_builder.Clear();
			_builder.Append($"{pos + (offset << 4):X9}");
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
				//Console.BackgroundColor = ConsoleColor.Black;
				//Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.White;
				Console.ForegroundColor = ConsoleColor.Black;
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

		public static OptionBox optionBox;

		static void CommandKey(ConsoleKeyInfo cki) {
			switch(cki.Key) {
			case ConsoleKey.G:
				optionBox = new GoToBox();
				break;
			case ConsoleKey.O:
				for(; ; ) {
					try {
						OpenFile();
						break;
					} catch { }
				}
				break;
			case ConsoleKey.S:
				for(; ; ) {
					try {
						SaveFile();
						break;
					} catch { }
				}
				break;
			case ConsoleKey.Y:
				if(doc.UndoIndex < doc.UndoCount) {
					position = doc.Redo();
					BlankBytes();
					ShowBytes();
				}
				break;
			case ConsoleKey.Z:
				if(doc.UndoIndex > 0) {
					position = doc.Undo();
					BlankBytes();
					ShowBytes();
				}
				break;
			}
		}

		static void ProcessInput(InputEventArgs e) {
			if(optionBox != null) {
				// Якщо наявне вікно опцій, перенаправити
				// туди ввід з клавіатури.
				optionBox.UserInput(e.Key);
				goto InputDone;
			}
			UnhighlightCursor();
			ConsoleKeyInfo cki = e.Key;
			if((cki.Modifiers & ConsoleModifiers.Control) != 0) {
				CommandKey(cki);
				goto InputDone;
			}
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
			case ConsoleKey.LeftArrow:
				MoveCursorLeft();
				break;
			case ConsoleKey.UpArrow:
				ScrollUp();
				break;
			case ConsoleKey.RightArrow:
				MoveCursorRight();
				break;
			case ConsoleKey.DownArrow:
				ScrollDown();
				break;
			case ConsoleKey.Insert:
				doc.InsertBytes(new byte[] { 0 }, position++);
				ShowBytes();
				break;
			case ConsoleKey.Delete:
				doc.DeleteBytes(1, position);
				ShowBytes();
				break;
			case ConsoleKey.D0:
			case ConsoleKey.D1:
			case ConsoleKey.D2:
			case ConsoleKey.D3:
			case ConsoleKey.D4:
			case ConsoleKey.D5:
			case ConsoleKey.D6:
			case ConsoleKey.D7:
			case ConsoleKey.D8:
			case ConsoleKey.D9:
				doc.OverwriteBytes(new byte[] {
					(byte)((doc[position] << 4) +
					cki.Key - ConsoleKey.D0)
					},
					position);
				ShowUpperBytes();
				break;
			case ConsoleKey.A:
			case ConsoleKey.B:
			case ConsoleKey.C:
			case ConsoleKey.D:
			case ConsoleKey.E:
			case ConsoleKey.F:
				doc.OverwriteBytes(new byte[] {
					(byte)((doc[position] << 4) +
					cki.Key - ConsoleKey.A + 10)
					},
					position);
				ShowUpperBytes();
				break;
			case ConsoleKey.NumPad0:
			case ConsoleKey.NumPad1:
			case ConsoleKey.NumPad2:
			case ConsoleKey.NumPad3:
			case ConsoleKey.NumPad4:
			case ConsoleKey.NumPad5:
			case ConsoleKey.NumPad6:
			case ConsoleKey.NumPad7:
			case ConsoleKey.NumPad8:
			case ConsoleKey.NumPad9:
				doc.OverwriteBytes(new byte[] {
					(byte)((doc[position] << 4) +
					cki.Key - ConsoleKey.NumPad0)
					},
					position);
				ShowUpperBytes();
				break;
			}

		InputDone:
			if(optionBox == null) {
				HighlightCursor();
			}
		}

		public static void GoTo(long pos) {
			position = pos > doc.Size ? doc.Size : pos;
			BlankBytes();
			ShowBytes();
		}
		public static void OpenFile() {
			if(oFileDialog.ShowDialog().GetValueOrDefault(false)) {
				doc = new Document(oFileDialog.FileName);
				BlankBytes();
				position = 0;
				ShowBytes();
			}
		}
		public static void SaveFile() {
			if(sFileDialog.ShowDialog().GetValueOrDefault(false)) {
				doc.Save(sFileDialog.FileName);
			}
		}

		static long position = 0;
		//static int selectionsize = 1;
		//static int cursordata;

		static void HighlightCursor() {
			int offset = (int)position & 15;
			int data = doc[position];
			lock(ConsoleUse) {
				Console.SetCursorPosition(10 + offset * 3, 2);
				//Console.BackgroundColor = ConsoleColor.White;
				//Console.ForegroundColor = ConsoleColor.Black;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.White;
				if(data == -1) {
					Console.Write("  ");
				} else {
					Console.Write($"{data:X2}");
				}
			}
		}
		static void UnhighlightCursor() {
			int offset = (int)position & 15;
			int data = doc[position];
			lock(ConsoleUse) {
				Console.SetCursorPosition(10 + offset * 3, 2);
				//Console.BackgroundColor = ConsoleColor.Black;
				//Console.ForegroundColor = ConsoleColor.White;
				Console.BackgroundColor = ConsoleColor.White;
				Console.ForegroundColor = ConsoleColor.Black;
				if(data == -1) {
					Console.Write("  ");
				} else {
					Console.Write($"{data:X2}");
				}
			}
		}

		static void MoveCursorLeft() {
			if(position != 0) {
				long oldpos = position;
				position--;
				if((oldpos & -16) != (position & -16)) {
					position += 16;
					ScrollUp();
				}
			}
		}
		static void MoveCursorRight() {
			if(position < doc.Size) {
				long oldpos = position;
				position++;
				if((oldpos & -16) != (position & -16)) {
					position -= 16;
					ScrollDown();
				}
			}
		}

		static void ScrollDown() {
			if(position + 16 < doc.Size) {
				position += 16;
				lock(ConsoleUse) {
					Console.MoveBufferArea(
						0, 3, RowSize, Console.WindowHeight - 4, 0, 2);
				}
				ShowLowerBytes();
			}
		}
		static void ScrollUp() {
			if(position > 15) {
				position -= 16;
				lock(ConsoleUse) {
					Console.MoveBufferArea(
						0, 2, RowSize, Console.WindowHeight - 4, 0, 3);
				}
				ShowUpperBytes();
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
			if(position != (doc.Size & -16)) {
				BlankBytes();
				position = doc.Size & -16;
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
			if(position + ((Console.WindowHeight - 4) << 4) >= doc.Size) {
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
					Console.Write(new string(' ', RowSize));
				}
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
			}
		}

		public static void RedrawScreen() {
			Console.Clear();
			ShowBytes();
		}
	}
}
