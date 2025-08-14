# RawPrint - Windows RAW Printing Utility

Lightweight console tool for sending raw print jobs to Windows printers via the spooler service.

## How It Works

Uses the Windows `winspool.drv` API to send RAW byte streams directly to printers. Bypasses printer drivers and sends data exactly as provided. The RAW stream MUST be compatible with your printer - ideally, fetched from your printer print queue.

## Installation

1. Download latest [release](https://github.com/yourusername/RawPrint/releases)
2. Place `RawPrint.exe` in your PATH or working directory

## Usage

```cmd
RawPrint <printer-name> <file-path>
```

Arguments:

| Argument     | Description                            |
|--------------|----------------------------------------|
| printer-name | Exact name of installed printer        |
| file-path    | Path to file containing raw print data |

Example:

```cmd
RawPrint "HP OfficeJet Pro" C:\printjobs\document.prn
```

Return Codes:

| Code | Meaning           |
|------|-------------------|
| 0    | Success           |
| 1    | Invalid arguments |
| 2    | File not found    |
| 3    | Printing error    |

## Why Regular Maintenance Matters

Inkjet printers are particularly susceptible to ink drying in the print head nozzles during periods of inactivity. This common issue leads to:

- Streaky or incomplete prints
- Wasted ink during cleaning cycles
- Potential hardware damage

While printing regular images can help, it consumes significant ink and provides limited diagnostic information about nozzle health.

Epson printers offer a specialized solution - the **Nozzle Check command** - a binary instruction that efficiently:

1. Uses minimal ink
2. Provides clear visual diagnostics
3. Maintains print head health

However, sending these manufacturer-specific RAW commands automatically (regularly) isn't implemented.

## Sample usage - Automated Nozzle Check Workflow

Here's how to implement scheduled nozzle checks using RawPrint:

### Step 1: Capture the Nozzle Check Command

1. Open your printer queue and **pause** the printer
2. Open Epson printer properties → Maintenance → Execute "Nozzle Check"
3. Navigate to `C:\Windows\System32\spool\PRINTERS`
4. Identify the latest `.SPL` file (spool job)
5. Copy this file (e.g., `NozzleCheck.spl`) to a permanent location

> ⚠️ **Note**: You may need Administrator privileges to access the spool directory

### Step 2: Identify exact printer name

- Either type in an exact printer name from printer properties
- OR use PowerShell cmdlet `Get-Printer | Format-List` and copy printer name from there for the needed printer.

### Step 3: Create a regular task via Windows Task Scheduler

- Start Task Scheduler
- Create a new Simple Task. Name it for yourself.
- Action:
  - Command: `RawPrint.exe`
  - Arguments: `"your printer name" NozzleCheck.spl`
  - Run folder: your folder with `RawPrint.exe` and `NozzleCheck.spl` files.

### Key Benefits

- Prevents ink drying with minimal ink consumption
- Early detection of clogged nozzles
- Fully automated maintenance
- Extends printer lifespan

## Building from Source

There's a GitHub pipeline set up to build project automatically, but if you want to build it yourself:

- Clone this repository.
- Go into project root folder.
- Run command: `dotnet publish -c Release`.
- Locate the executable in `bin\Release\net8.0\win-x64\publish\`.

The build requires .NET 8.0 SDK.
