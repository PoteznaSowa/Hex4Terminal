using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Hex4Terminal {
	class Document {
		FileStream Stream;
		public string Name {
			get; private set;
		}
		public long Size {
			get; private set;
		}
		public bool Modified {
			get; private set;
		}

		readonly List<Region> undobuffer = new List<Region>(8);
		public int UndoIndex {
			get; private set;
		}
		public int UndoCount {
			get => undobuffer.Count;
		}

		public Document() {
			Stream = null;
			Name = "untitled";
		}
		public Document(string path) {
			Stream = File.OpenRead(path);
			Size = Stream.Length;
			Name = Path.GetFileName(Stream.Name);
		}
		~Document() {
			if(Stream != null) {
				Stream.Dispose();
			}
		}

		public int this[long position] => ReadFromUndo(position, UndoIndex);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		int ReadFromUndo(long position, int index) {
			if(index == 0) {
				if(Stream != null) {
					Stream.Seek(position, SeekOrigin.Begin);
					return Stream.ReadByte();
				} else {
					return -1;
				}
			}

			index--;
			Region r = undobuffer[index];
			if(position < r.Position) {
				return ReadFromUndo(position, index);
			} else if(r is RegionDeleted) {
				return ReadFromUndo(position + r.Size, index);
			} else if(r is DataRegion dr) {
				return position < r.Position + r.Size ?
					dr[(int)(position - r.Position)] :
					ReadFromUndo(position - r.SizeDelta, index);
			} else {
				//throw new Exception("Зіпсований undo-буфер.");
				return -1;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte[] Read(long position, int count) {
			List<byte> bytes = new List<byte>(16);
			int data;
			int num = 0;
			while(-1 != (data = this[position + num]) && count-- != 0) {
				bytes.Add((byte)data);
				num++;
			}
			return bytes.ToArray();
		}

		public void InsertBytes(byte[] data, long position) {
			Modified = true;
			ClearUndoneChanges();
			undobuffer.Add(new RegionInserted(data, position));
			UndoIndex++;
			Size += data.Length;
		}
		public void DeleteBytes(long count, long position) {
			Modified = true;
			ClearUndoneChanges();
			undobuffer.Add(new RegionDeleted(count, position));
			UndoIndex++;
			Size -= count;
		}
		public void OverwriteBytes(byte[] data, long position) {
			Modified = true;
			ClearUndoneChanges();
			DeleteDuplicateUndo(data.Length, position, typeof(RegionOverwritten));
			if(position + data.Length > Size) {
				byte[] head = new byte[Size - position];
				Array.Copy(data, head, head.Length);
				OverwriteBytes(head, position);
				byte[] tail = new byte[position + data.Length - Size];
				Array.Copy(data, Size - position, tail, 0, tail.Length);
				InsertBytes(tail, position);
			} else {
				undobuffer.Add(new RegionOverwritten(data, position));
			}
			UndoIndex++;
		}
		void DeleteDuplicateUndo(long size, long position, Type type) {
			if(UndoCount > 0) {
				Region r = undobuffer[undobuffer.Count - 1];
				if(r.GetType() == type && r.Position == position && r.Size == size) {
					undobuffer.RemoveAt(undobuffer.Count - 1);
					UndoIndex--;
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ClearUndoneChanges() {
			if(UndoIndex < undobuffer.Count) {
				undobuffer.RemoveRange(UndoIndex, undobuffer.Count - UndoIndex);
			}
		}

		public long Undo() {
			Modified = true;
			return UndoIndex > 0
				? undobuffer[--UndoIndex].Position
				: throw new InvalidOperationException("Нема чого скасовувати!");
		}

		public long Redo() {
			if(UndoIndex < undobuffer.Count) {
				Modified = true;
				Region r = undobuffer[UndoIndex++];
				return r is DataRegion ? r.Position + r.Size : r.Position;
			}
			throw new InvalidOperationException("Нема чого повертати!");
		}

		public void Save() {
			Modified = false;
			int num = 0;
			string name = Path.GetFileNameWithoutExtension(Stream.Name);
			string ext = Path.GetExtension(Stream.Name);
			string path = new FileInfo(Stream.Name).DirectoryName;
			while(File.Exists($@"{path}\{name}{num}{ext}")) {
				num++;
			}
			string bakname = $@"{path}\{name}{num}{ext}";
			FileStream file = File.Create(bakname);
			for(long i = 0; i < Size; i++) {
				file.WriteByte((byte)this[i]);
			}
			file.Flush(true);
			file.Dispose();
			Stream.Dispose();
			string newname = $@"{path}\{name}{ext}";
			File.Move(bakname, newname);
			Stream = File.OpenRead(newname);
			UndoIndex = 0;
			ClearUndoneChanges();
		}
		public void Save(string path) {
			Modified = false;
			FileStream file = File.Create(path);
			for(long i = 0; i < Size; i++) {
				file.WriteByte((byte)this[i]);
			}
			file.Flush(true);
			file.Dispose();
		}
	}
}
