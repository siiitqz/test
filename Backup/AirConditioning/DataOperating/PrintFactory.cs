using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;


/// <summary>
/// 打印基础类
/// </summary>
public class PrintFactory
{
    public const short FILE_ATTRIBUTE_NORMAL = 0x80;
    public const short INVALID_HANDLE_VALUE = -1;
    public const uint GENERIC_READ = 0x80000000;
    public const uint GENERIC_WRITE = 0x40000000;
    public const uint CREATE_NEW = 1;
    public const uint CREATE_ALWAYS = 2;
    public const uint OPEN_EXISTING = 3;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess,
        uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition,
        uint dwFlagsAndAttributes, IntPtr hTemplateFile);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="receiptText"></param>
    public static void SendCMDToLPT(string lptName, Byte[] buffer)
    {
        IntPtr ptr = CreateFile(lptName, GENERIC_WRITE, 0,
                 IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

        if (ptr.ToInt32() == -1)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }
        else
        {
            FileStream lpt = new FileStream(ptr, FileAccess.ReadWrite);
            lpt.Write(buffer, 0, buffer.Length);
            lpt.Close();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="receiptText"></param>
    public static void PrintCMDToLPT(string lptName, string receiptText)
    {
        IntPtr ptr = CreateFile(lptName, GENERIC_WRITE, 0,
                 IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

        if (ptr.ToInt32() == -1)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }
        else
        {
            FileStream lpt = new FileStream(ptr, FileAccess.ReadWrite);
            //StreamWriter sw = new StreamWriter(lpt, System.Text.Encoding.Default);
            //sw.WriteLine(receiptText);
            //sw.Close();
            //lpt.Close();

            Byte[] buffer = System.Text.Encoding.Default.GetBytes(receiptText);
            lpt.Write(buffer, 0, buffer.Length);
            lpt.Close();
        }
    }
}
