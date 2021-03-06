﻿using CommandLine;

namespace eRecruiter.ApplicantImport
{
    public class CommandLineArguments
    {
        public CommandLineArguments()
        {
            ContinueOnWarnings = false;
        }

        [Option('c', "config", Required = true, HelpText = "The (relative or absolute) path to the configuration JSON file.")]
        public string ConfigurationFile { get; set; }

        [Option('f', "file", Required = true, HelpText = "The (relative or absolute) path to the CSV file to import from.")]
        public string CsvFile { get; set; }

        [Option('w', "continueOnWarnings", Required = false, HelpText = "Whether or not to automatically continue, even when warnings have been found.")]
        public bool ContinueOnWarnings { get; set; }

        [Option('s', "generateCsvStub", Required = false, HelpText = "Whether or not to build an empty CSV file that contains all columns specified in the configuration.")]
        public bool GenerateCsvStub { get; set; }

        [Option('l', "logFile", Required = false, HelpText = "The (relative or absolute) path to a file to log all console output to.")]
        public string LogFile { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            Program.WriteError("Usage:");
            Program.WriteError("eRecruiter.ApplicantImport.exe --config=MyConfig.json --file=Applicants.csv [--continueOnWarning] [--generateCsvStub] [--logFile=log.txt]");
            return "";
        }
    }
}
