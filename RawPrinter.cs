using System.Drawing.Printing;
using System.Runtime.InteropServices;

public static class RawPrinter
{
    [StructLayout(LayoutKind.Sequential)]
    private struct DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)]
        public string pDocName;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)]
        public string pDataType;
    }

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool OpenPrinter(
        string szPrinter,
        out IntPtr hPrinter,
        IntPtr pd);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool StartDocPrinter(
        IntPtr hPrinter,
        int level,
        ref DOCINFOA di);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool WritePrinter(
        IntPtr hPrinter,
        IntPtr pBytes,
        int dwCount,
        out int dwWritten);

    public static void SendBytesToPrinter(string printerName, byte[] bytes)
    {
        IntPtr hPrinter = IntPtr.Zero;
        DOCINFOA di = new DOCINFOA();
        int dwWritten = 0;
        bool success = false;
        int errorCode = 0;

        di.pDocName = "Raw Print Job";
        di.pDataType = "RAW";

        try
        {
            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero))
            {
                errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(
                    $"Could not open printer. Error code: {errorCode}");
            }

            if (!StartDocPrinter(hPrinter, 1, ref di))
            {
                errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(
                    $"Could not start document. Error code: {errorCode}");
            }

            if (!StartPagePrinter(hPrinter))
            {
                errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(
                    $"Could not start page. Error code: {errorCode}");
            }

            IntPtr pUnmanagedBytes = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, pUnmanagedBytes, bytes.Length);
            success = WritePrinter(
                hPrinter,
                pUnmanagedBytes,
                bytes.Length,
                out dwWritten);
            Marshal.FreeCoTaskMem(pUnmanagedBytes);

            if (!success)
            {
                errorCode = Marshal.GetLastWin32Error();
                throw new ApplicationException(
                    $"Write error. Sent {dwWritten}/{bytes.Length} bytes. Error code: {errorCode}");
            }

            EndPagePrinter(hPrinter);
            EndDocPrinter(hPrinter);
        }
        finally
        {
            if (hPrinter != IntPtr.Zero)
                ClosePrinter(hPrinter);
        }
    }
}
