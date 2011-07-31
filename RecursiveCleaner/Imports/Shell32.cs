using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace RecursiveCleaner.Imports
{
    class Shell32
    {
        public enum FO_Func : uint
        {
            FO_MOVE = 0x0001,
            FO_COPY = 0x0002,
            FO_DELETE = 0x0003,
            FO_RENAME = 0x0004,
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 2)]
        public struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            public FO_Func wFunc;
            public IntPtr pFrom;
            public IntPtr pTo;
            public ushort fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpszProgressTitle;

        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        public static extern int SHFileOperation([In] ref SHFILEOPSTRUCT lpFileOp);

        public const ushort FOF_ALLOWUNDO = 0x0040;
        public const ushort FOF_SILENT = 0x0004;
        public const ushort FOF_NOCONFIRMATION = 0x0010;
        public const ushort FOF_NOCONFIRMMKDIR = 0x0200;
        public const ushort FOF_NOERRORUI = 0x0400;
        public const ushort FOF_NO_UI = (FOF_SILENT | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_NOCONFIRMMKDIR);
    }
}
