using System;
using System.IO;
using System.Text;
using static System.Console;

namespace Hex4Terminal {
	static class Screen {
		// Клас головного інтерфейсу програми.

		const int ScrWdth = 80;  // Ширина екрану.


		// Ось як інтерфейс має виглядати:

		//  File  Edit  Search  Options
		//╔════════╤═══════════════════════════════════════════════╤════════════════
		//║00000000│00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F│CP1251
		//╟────────┼───────────────────────────────────────────────┼────────────────
		//║00000000│
		//║00000010│
		//║00000020│
		//║00000030│
		//║00000040│
		//║00000050│
		//║00000060│
		//║00000070│
		//║00000080│
		//║00000090│
		//║000000A0│
		//║000000B0│
		//║000000C0│
		//║000000D0│
		//║000000E0│
		//║000000F0│
		//
		//
		//
		//
		// F1 Help  F2 Save  F3 Open  

		static Screen() {

		}

		public static void Initialize() {
			BackgroundColor = ConsoleColor.Black;
			Clear();
			SetBufferSize(ScrWdth, WindowHeight);
		}

		public static void Update() {
			
		}
		public static void Highlight(long position) {
			// Підсвітити поточну комірку файлу.

		}
		public static void Highlight(long position, long size) {
			// Підсвітити декілька комірок підряд.

		}
		public static void UpdateStatusBar(string text) {
			
		}
	}
}
