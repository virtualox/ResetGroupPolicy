# Reset Group Policy (C#)

This C# console application resets the group policies on a Windows client to their default settings. It also writes an informational event to the Event Viewer.

## Requirements

- Windows operating system
- .NET 6 runtime installed on the target machine
- Administrator privileges

## Building

1. Open the solution file in Visual Studio 2022 or later.
2. Build the solution in Release mode.

Alternatively, you can build the solution from the command line using the following command:

```sh
dotnet build --configuration Release
```

## Usage
1. Open an elevated Command Prompt or PowerShell console (Run as Administrator).
2. Navigate to the directory containing the compiled executable.
3. Run the application using the following command:
```sh
ResetGroupPolicy.exe [--verbose] [--help]
```

## Command-line options
- `--verbose`: Enable verbose logging. When this option is used, the application will print detailed information about its progress to the console.
- `--help`: Show a help message with a description of the command-line options.

## Example
To run the application with verbose logging, use the following command:

```sh
ResetGroupPolicy.exe --verbose
```
