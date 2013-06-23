/*

    IcoConverter - Extracts images from .ICO files
    Copyright (C) 2013 - Daniel Lemke <lemked@web.de>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

 */

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

        [Option('d', "destination", Required = false, HelpText = "Destination directory.")]
        public string DestinationDirectory { get; set; }
    }
}
