using System.Text;
using CommandLine;

namespace IcoConverter
{
    class CommandLineOptions
    {
        [Option('h', "help", Required = false, HelpText = "Show usage information.")]
        public bool ShowHelp { get; set; }

        [Option('i', "input", Required = false, HelpText = ".ICO file or directory with .ICO files.")]
        public string Input { get; set; }

        [Option('f', "format", Required = false, HelpText = "File format.")]
        public string OutputFileFormat { get; set; }

        [Option('d', "dest", Required = false, HelpText = "Destination directory.")]
        public string DestinationDirectory { get; set; }
    }
}
