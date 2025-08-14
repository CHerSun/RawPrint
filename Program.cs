public class Program
{
    static readonly HashSet<string> ValidHelpArgs = new HashSet<string> { "-h", "--help", "/h", "/?" };

    static int Main(string[] args)
    {
        if (args.Length == 1 && ValidHelpArgs.Contains(args[0]))
        {
            ShowHelp();
            return 0;
        }

        if (args.Length != 2)
        {
            Console.WriteLine("Error: Incorrect number of arguments");
            ShowHelp();
            return 1;
        }

        string printerName = args[0];
        string filePath = args[1];

        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Error: File not found - {filePath}");
                return 2;
            }

            byte[] fileBytes = File.ReadAllBytes(filePath);
            RawPrinter.SendBytesToPrinter(printerName, fileBytes);
            Console.WriteLine($"Document sent to printer: {printerName}");
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Printing error: {ex.Message}");
            return 3;
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine($"RawPrint v{typeof(Program).Assembly.GetName().Version} - direct RAW printing utility for Windows.");
        Console.WriteLine("");
        Console.WriteLine("Usage:");
        Console.WriteLine("\tRawPrint <printer-name> <file-path>");
        Console.WriteLine("");
        Console.WriteLine("Example:");
        Console.WriteLine("\tRawPrint \"My Printer\" C:\\Files\\document.prn");
        Console.WriteLine("");
        Console.WriteLine("Notes:");
        Console.WriteLine("  - Printer name must match exactly with installed printer. Use `Get-Printer` PowerShell cmdlet to get printer names.");
        Console.WriteLine("  - File will be sent to printer without modification (RAW).");
    }
}
