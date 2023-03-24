using System;
using System.IO;
using System.Diagnostics;
using System.Security.Principal;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;

namespace ResetGroupPolicy
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configure the EventLog logger
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddEventLog(settings =>
                {
                    settings.SourceName = "ResetGroupPolicy";
                });
            });

            ILogger logger = loggerFactory.CreateLogger<Program>();

            if (args.Contains("--help") || args.Contains("-h") || args.Contains("-?"))
            {
                ShowHelp();
                return;
            }

            // Check if the program is running with administrative privileges
            bool isAdmin;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }

            if (!isAdmin)
            {
                Console.WriteLine("This program must be run with administrative privileges.");
                return;
            }

            bool verbose = args.Contains("--verbose") || args.Contains("-v");

            string groupPolicyUsersPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"System32\GroupPolicyUsers");
            string groupPolicyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"System32\GroupPolicy");

            try
            {
                if (Directory.Exists(groupPolicyUsersPath))
                {
                    if (verbose) Console.WriteLine($"Deleting directory: {groupPolicyUsersPath}");
                    Directory.Delete(groupPolicyUsersPath, true);
                }
                else
                {
                    if (verbose) Console.WriteLine($"Directory not found: {groupPolicyUsersPath}");
                }

                if (Directory.Exists(groupPolicyPath))
                {
                    if (verbose) Console.WriteLine($"Deleting directory: {groupPolicyPath}");
                    Directory.Delete(groupPolicyPath, true);
                }
                else
                {
                    if (verbose) Console.WriteLine($"Directory not found: {groupPolicyPath}");
                }

                if (verbose) Console.WriteLine("Running gpupdate /force");
                Process gpupdate = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "gpupdate",
                        Arguments = "/force",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };

                gpupdate.Start();
                gpupdate.WaitForExit();

                // Log success event to Event Viewer
                logger.LogInformation("Group policy reset successfully.");
            }
            catch (Exception ex)
            {
                // Log error event to Event Viewer
                logger.LogError(ex, "Group policy reset failed.");
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("ResetGroupPolicy");
            Console.WriteLine("Resets the local group policy settings to their defaults.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --help, -h, -?    Show this help message and exit.");
            Console.WriteLine("  --verbose, -v     Enable verbose logging.");
        }
    }
}