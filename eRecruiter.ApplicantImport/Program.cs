using System;

namespace eRecruiter.ApplicantImport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Write("Good Day. I hope you are doing well?");
            Write("");

            #region Command Line Arguments
            Write("Reading and verifying command line arguments ...");
            bool hasErrors;
            var commandLineArguments = new CommandLineService().ReadAndVerify(args, out hasErrors);
            if (hasErrors)
            {
                Write("Exiting ...");
                Environment.Exit(1);
            }
            Write("Command line arguments seem okay.");
            Write("");
            #endregion

            #region Configuration File
            Write("Reading and verifying configuration file ...");
            bool hasWarnings;
            var configuration = new ConfigurationService(commandLineArguments).ReadAndVerify(out hasErrors, out hasWarnings);
            if (hasErrors)
            {
                Write("Exiting ...");
                Environment.Exit(2);
            }
            if (hasWarnings)
                ConfirmWarnings(commandLineArguments);
            else
                Write("Configuration file seems okay.");
            Write("");
            #endregion

            #region CSV file
            Write("Reading and verifying CSV file ...");
            var csv = new CsvService(commandLineArguments, configuration).ReadAndVerify(out hasErrors, out hasWarnings);
            if (hasErrors)
            {
                Write("Exiting ...");
                Environment.Exit(3);
            }
            if (hasWarnings)
                ConfirmWarnings(commandLineArguments);
            else
                Write("CSV file seems okay.");
            Write("");
            #endregion

            #region Import
            new ImportService(configuration, csv).RunImport(out hasErrors);
            if (hasErrors)
            {
                Write("Exiting ...");
                Environment.Exit(4);
            }
            Write("");
            #endregion

            Write("Everything done. Have a nice day.");
            Environment.Exit(0);
        }

        private static void ConfirmWarnings(CommandLineArguments commandLineArguments)
        {
            Write("There are warnings (see above). These may or may not be problematic during the import.");
            if (!commandLineArguments.ContinueOnWarnings)
            {
                Write("If you want to continue despite these warnings, confirm by typing YES. Press < Enter > to exit.");
                var confirm = Console.ReadLine();
                if (confirm != "YES")
                {
                    Write("Exiting ...");
                    Environment.Exit(3);
                }
            }
        }

        #region Console Output
        public static void Write(string line)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(line);
        }

        public static void WriteError(params string[] lines)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var line in lines)
                Console.WriteLine("- " + line);
        }

        public static void WriteWarning(params string[] lines)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var line in lines)
                Console.WriteLine("- " + line);
        }
        #endregion
    }
}
