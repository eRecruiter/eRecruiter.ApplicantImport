using eRecruiter.Utilities;
using JetBrains.Annotations;
using System;
using System.IO;

namespace eRecruiter.ApplicantImport
{
    public class CommandLineService
    {
        [NotNull]
        public CommandLineArguments ReadAndVerify([NotNull] string[] commandLineArgs, out bool requiresExit)
        {
            requiresExit = false;
            var options = new CommandLineArguments();
            if (CommandLine.Parser.Default.ParseArguments(commandLineArgs, options))
            {
                // make relative paths absolute
                if (!Path.IsPathRooted(options.ConfigurationFile))
                    options.ConfigurationFile = Path.Combine(Environment.CurrentDirectory, options.ConfigurationFile);
                if (!Path.IsPathRooted(options.CsvFile))
                    options.CsvFile = Path.Combine(Environment.CurrentDirectory, options.CsvFile);

                // check if files exist
                if (!File.Exists(options.ConfigurationFile))
                {
                    Program.WriteError(string.Format("Configuration file {0} not found.", options.ConfigurationFile));
                    requiresExit = true;
                }

                if (!options.GenerateCsvStub && !File.Exists(options.CsvFile))
                {
                    Program.WriteError(string.Format("CSV file {0} not found.", options.CsvFile));
                    requiresExit = true;
                }
                else if (options.GenerateCsvStub && File.Exists(options.CsvFile))
                {
                    Program.WriteError(string.Format("CSV stub file {0} already exists.", options.CsvFile));
                    requiresExit = true;
                }
            }
            else
            {
                Program.WriteError("Make sure you specified all required arguments.");
                requiresExit = true;
            }

            return options;
        }
    }
}
