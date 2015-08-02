using System.Diagnostics;
using System.Runtime.InteropServices;
using System;

namespace CSGoTrainer
{
	public class ProcessMemoryHandler
	{

		#region DLL Imports

		[DllImport("kernel32")]
		private static extern int OpenProcess(int accessType, int inheritHandle, int processId);

		[DllImport("kernel32", EntryPoint = "WriteProcessMemory")]
		private static extern byte WriteProcessMemoryByte(int handle, int address, ref byte value, int size, ref int bytesWritten);

		[DllImport("kernel32", EntryPoint = "WriteProcessMemory")]
		private static extern int WriteProcessMemoryInteger(int handle, int address, ref int value, int size, ref int bytesWritten);

		[DllImport("kernel32", EntryPoint = "WriteProcessMemory")]
		private static extern float WriteProcessMemoryFloat(int handle, int address, ref float value, int size, ref int bytesWritten);

		[DllImport("kernel32", EntryPoint = "WriteProcessMemory")]
		private static extern double WriteProcessMemoryDouble(int handle, int address, ref double value, int size, ref int bytesWritten);

		[DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
		private static extern byte ReadProcessMemoryByte(int handle, int address, ref byte value, int size, ref int bytesRead);

		[DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
		private static extern int ReadProcessMemoryInteger(int handle, int address, ref int value, int size, ref int bytesRead);

		[DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
		private static extern float ReadProcessMemoryFloat(int handle, int address, ref float value, int size, ref int bytesRead);

		[DllImport("kernel32", EntryPoint = "ReadProcessMemory")]
		private static extern double ReadProcessMemoryDouble(int handle, int address, ref double value, int size, ref int bytesRead);

		[DllImport("kernel32.dll")]
		public static extern Int32 ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [In, Out] byte[] buffer,
			UInt32 size, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32")]
		private static extern int CloseHandle(int handle);

		[DllImport("user32")]
		private static extern int FindWindow(string sClassName, string sAppName);

		[DllImport("user32")]
		private static extern int GetWindowThreadProcessId(int hwnd, out int processId);

		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenThread(ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

		/*[DllImport("kernel32.dll")]
		private static extern uint SuspendThread(IntPtr hThread);*/

		[DllImport("kernel32.dll")]
		private static extern int ResumeThread(IntPtr hThread);

		/*private void SuspendProcess(int PID)
		{
			Process proc = Process.GetProcessById(PID);

			if (proc.ProcessName == string.Empty)
				return;

			foreach (ProcessThread pT in proc.Threads)
			{
				IntPtr pOpenThread = OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint) pT.Id);

				if (pOpenThread == IntPtr.Zero)
				{
					break;
				}

				SuspendThread(pOpenThread);
			}
		}*/

		public void ResumeProcess(int pid)
		{
			Process proc = Process.GetProcessById(pid);

			if (proc.ProcessName == string.Empty)
				return;

			foreach (ProcessThread pT in proc.Threads)
			{
				IntPtr pOpenThread = OpenThread(ThreadAccess.SuspendResume, false, (uint) pT.Id);

				if (pOpenThread == IntPtr.Zero)
				{
					break;
				}

				ResumeThread(pOpenThread);
			}
		}

		[Flags]
		public enum ThreadAccess
		{
			Terminate = 0x0001,
			SuspendResume = 0x0002,
			GetContext = 0x0008,
			SetContext = 0x0010,
			SetInformation = 0x0020,
			QueryInformation = 0x0040,
			SetThreadToken = 0x0080,
			Impersonate = 0x0100,
			DirectImpersonation = 0x0200
		}

		public const int ProcessVmRead = (0x0010);
		public const int ProcessVmWrite = (0x0020);
		public const int ProcessVmOperation = (0x0008);
		public const int ProcessVmAllAccess = 0x1F0FFF;

		#endregion

		protected Process _process;

		public Process Process
		{
			get { return _process; }
			set { _process = value; Open(); }
		}

		private IntPtr _processHandle;

		public bool Alive
		{
			get { return (_processHandle != IntPtr.Zero && Process != null); }
		}

		public ProcessMemoryHandler(Process process)
		{
			Process = process;
			_processHandle = IntPtr.Zero;
		}

		public void Open()
		{
			if (Process != null)
			{
				_processHandle = (IntPtr) OpenProcess(ProcessVmAllAccess, 1, _process.Id);
			}
		}

		public void Close()
		{
			CloseHandle((int)_processHandle);
		}

		#region Read

		public Byte ReadInt8(int address)
		{
			byte value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int bytes = 0;
				ReadProcessMemoryByte((int)_processHandle, address, ref value, 1, ref bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		public Int16 ReadInt16(int address)
		{
			Int16 value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int int32Value = ReadInt32(address);
				byte[] int32ValueBytes = BitConverter.GetBytes(int32Value);
				value = BitConverter.ToInt16(int32ValueBytes, 0);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		public Int32 ReadInt32(int address)
		{
			Int32 value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int bytes = 0;
				ReadProcessMemoryInteger((int)_processHandle, address, ref value, 4, ref bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		public Int64 ReadInt64(int address)
		{
			Int64 value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				byte[] bytes8 = ReadByteArray(address, 8);
				value = BitConverter.ToInt64(bytes8, 0);
				//ReadProcessMemoryInteger(Handle, Address, ref Value, 4, ref Bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		public float ReadFloat(int address)
		{
			float value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int bytes = 0;
				ReadProcessMemoryFloat((int)_processHandle, address, ref value, 4, ref bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		public double ReadDouble(int address)
		{
			double value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int bytes = 0;
				ReadProcessMemoryDouble((int)_processHandle, address, ref value, 8, ref bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		#endregion

		public int ReadMultiLevelPointer(int memoryAddress, uint bytesToRead, Int32[] offsetList)
		{
			if (!Alive)
			{
				return 0;
			}

			byte[] btBuffer = new byte[bytesToRead];

			int pointerAddy = memoryAddress;
			for (int i = 0; i < (offsetList.Length); i++)
			{
				IntPtr lpOutStorage;
				if (i == 0)
				{
					ReadProcessMemory(_processHandle, (IntPtr)(pointerAddy), btBuffer, (uint)btBuffer.Length, out lpOutStorage);
				}
				pointerAddy = (BitConverter.ToInt32(btBuffer, 0) + offsetList[i]);
				ReadProcessMemory(_processHandle, (IntPtr)(pointerAddy), btBuffer, (uint)btBuffer.Length, out lpOutStorage);
			}
			return pointerAddy;
		}

		#region ReadPointer

		public Byte ReadPointerInt8(int pointer, int[] offset)
		{
			byte value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int) _processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				ReadProcessMemoryByte((int) _processHandle, pointer, ref value, 2, ref bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		public Int16 ReadPointerInt16(int pointer, int[] offset)
		{
			try
			{
				if (!Alive)
				{
					return 0;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int) _processHandle, pointer, ref pointer, 2, ref bytes);
					pointer += i;
				}
				ReadInt16(pointer);
				//ReadProcessMemoryInteger((int)Handle, Pointer, ref Value, 4, ref Bytes);
			}
			catch
			{
				// ignored
			}
			return 0;
		}

		public int ReadPointerInt32(int pointer, int[] offset)
		{
			int value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int) _processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				ReadProcessMemoryInteger((int) _processHandle, pointer, ref value, 4, ref bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		public Int64 ReadPointerInt64(int pointer, int[] offset)
		{
			Int64 value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int) _processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				value = ReadInt64(pointer);
				//ReadProcessMemoryInteger((int)Handle, Pointer, ref Value, 4, ref Bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		public float ReadPointerFloat(int pointer, int[] offset)
		{
			float value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int) _processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				ReadProcessMemoryFloat((int) _processHandle, pointer, ref value, 4, ref bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		public double ReadPointerDouble(int pointer, int[] offset)
		{
			double value = 0;
			try
			{
				if (!Alive)
				{
					return value;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int) _processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				ReadProcessMemoryDouble((int) _processHandle, pointer, ref value, 8, ref bytes);
			}
			catch
			{
				// ignored
			}
			return value;
		}

		#endregion

		#region Write

		public void WriteInt8(int address, byte value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				WriteProcessMemoryByte((int)_processHandle, address, ref value, 1, ref bytes);
			}
			catch
			{
				// ignored
			}
		}

		public void WriteInt16(int address, Int16 value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				WriteInt16(address, value);
				//WriteProcessMemoryByte(Handle, Address, ref Value, 2, ref Bytes);
			}
			catch
			{
				// ignored
			}
		}

		public void WriteInt32(int address, int value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				WriteProcessMemoryInteger((int)_processHandle, address, ref value, 4, ref bytes);
			}
			catch
			{
				// ignored
			}
		}

		public void WriteInt64(int address, Int64 value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				byte[] bytes8 = BitConverter.GetBytes(value);
				WriteByteArray(address, bytes8);
				//WriteProcessMemoryInteger(Handle, Address, ref Value, 4, ref Bytes);

			}
			catch
			{
				// ignored
			}
		}

		public void WriteFloat(int address, float value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				WriteProcessMemoryFloat((int)_processHandle, address, ref value, 4, ref bytes);
			}
			catch
			{
				// ignored
			}
		}

		public void WriteDouble(int address, double value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				WriteProcessMemoryDouble((int)_processHandle, address, ref value, 8, ref bytes);
			}
			catch
			{
				// ignored
			}
		}

		#endregion

		#region WritePointer

		public void WritePointerInt8(int pointer, int[] offset, byte value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int)_processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				WriteProcessMemoryByte((int)_processHandle, pointer, ref value, 1, ref bytes);
			}
			catch
			{
				// ignored
			}
		}

		public void WritePointerInt16(int pointer, int[] offset, Int16 value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int)_processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				WriteInt16(pointer, value);
				//WriteProcessMemoryInteger(Handle, Pointer, ref Value, 4, ref Bytes);
			}
			catch
			{
				// ignored
			}
		}

		public void WritePointerInt32(int pointer, int[] offset, int value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int)_processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				WriteProcessMemoryInteger((int)_processHandle, pointer, ref value, 4, ref bytes);
			}
			catch
			{
				// ignored
			}
		}

		public void WritePointerInt64(int pointer, int[] offset, Int64 value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int)_processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				WriteInt64(pointer, value);
				//WriteProcessMemoryInteger(Handle, Pointer, ref Value, 4, ref Bytes);
			}
			catch
			{
				// ignored
			}
		}

		public void WritePointerFloat(int pointer, int[] offset, float value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int)_processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				WriteProcessMemoryFloat((int)_processHandle, pointer, ref value, 4, ref bytes);
			}
			catch
			{
				// ignored
			}
		}

		public void WritePointerDouble(int pointer, int[] offset, double value)
		{
			try
			{
				if (!Alive)
				{
					return;
				}
				int bytes = 0;
				foreach (int i in offset)
				{
					ReadProcessMemoryInteger((int)_processHandle, pointer, ref pointer, 4, ref bytes);
					pointer += i;
				}
				WriteProcessMemoryDouble((int)_processHandle, pointer, ref value, 8, ref bytes);
			}
			catch
			{
				// ignored
			}
		}

		#endregion

		#region Others

		public byte[] ReadByteArray(int address, int count)
		{
			if (count > 0)
			{
				try
				{
					byte[] value = new byte[count];
					for (int i = 0; i < count; i++)
					{
						value[i] = ReadInt8(address + i);
					}
					return value;
				}
				catch
				{
					// ignored
				}
			}
			return null;
		}

		public void WriteByteArray(int address, byte[] bytes)
		{
			for (int i = 0; i < bytes.Length; i++)
			{
				WriteInt8(address + i, bytes[i]);
			}
		}

		public string ReadStringUntilNull(int address)
		{
			string value = "";
			//bool endOfString = false;
			int counter = 0;
			while (true)
			{
				if (ReadInt8(address + counter) > 0)
				{
					value += (char) ReadInt8(address + counter);
				}
				else
				{
					return value;
				}
				counter++;
			}
			//return value;
		}

		public void WriteString(int address, string value)
		{
			if (value != null)
			{
				int counter = 0;
				foreach (char chr in value)
				{
					WriteInt8(address + counter, (byte) chr);
					counter++;
				}
			}
		}

		public void WriteNoPs(int address, int count)
		{
			for (int i = 0; i < count; i++)
			{
				WriteInt8(address + i, 0x90);
			}
		}

		public bool GetHandleByTitle(string windowTitle)
		{
			try
			{
				int proc;
				int hwnd = FindWindow(null, windowTitle);
				GetWindowThreadProcessId(hwnd, out proc);
				int handle = OpenProcess(ProcessVmAllAccess, 0, proc);
				if (handle != 0)
				{
					return true;
				}
			}
			catch
			{
				// ignored
			}
			return false;
		}

		public void WriteCodeInjection(int injectAddress, byte[] injectCode, int codeCaveAddress, byte[] codeCaveCode)
		{
			try
			{
				WriteByteArray(injectAddress, injectCode);
				WriteByteArray(codeCaveAddress, codeCaveCode);
			}
			catch
			{
				// ignored
			}
		}

		#endregion
	}
}