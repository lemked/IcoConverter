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

using System;
using System.IO;
using System.Windows.Media.Imaging;
using CommandLine;

namespace IcoConverter
{
    public enum FileFormat
    {
       Unknown, PNG, JPEG, BMP, GIF, TIFF, WMP
    }

    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("IcoConverter - Extracts images from .ICO files");
            Console.WriteLine("Copyright (C) 2013 - Daniel Lemke <lemked@web.de>");
            Console.WriteLine();

            if (args.Length.Equals(0))
            {
                ShowUsage();
                return;
            }

            string input = String.Empty;
            string destinationDir = String.Empty;
            
            // Use PNG as default output file format.
            FileFormat fileFormat = FileFormat.PNG;

            //Parse Command Line
            var commandLineOptions = new CommandLineOptions();
            var parser = new Parser();
            if (parser.ParseArguments(args, commandLineOptions))
            {
                if (commandLineOptions.ShowHelp)
                {
                    ShowUsage();
                    return;
                }

                if (!String.IsNullOrEmpty(commandLineOptions.Input))
                {
                    input = commandLineOptions.Input;
                }

                if (!String.IsNullOrEmpty(commandLineOptions.DestinationDirectory))
                {
                    destinationDir = commandLineOptions.DestinationDirectory;
                }

                // Parse the file format from the command line
                if (!String.IsNullOrEmpty(commandLineOptions.OutputFileFormat))
                {
                    if (ParseFileFormat(commandLineOptions.OutputFileFormat) == FileFormat.Unknown)
                    {
                        Console.WriteLine("{0} is not a supported output file format. Supported formats are PNG, BMP, JPEG, GIF, TIFF and WMP.", commandLineOptions.OutputFileFormat);
                        return;
                    }
                    fileFormat = ParseFileFormat(commandLineOptions.OutputFileFormat);
                }
            }
            else
            {
                ShowUsage();
                return;
            }
            
            if (String.IsNullOrEmpty(input))
            {
                input = args[0];
                if (input.StartsWith("-"))
                {
                    Console.WriteLine("Please specify a .ICO file or a directory which contains .ICO files.");
                    return;
                }
            }

            if (!File.Exists(input) && !Directory.Exists(input))
            {
                Console.WriteLine(".ICO file or directory not found.");
                return;
            }

            if (!String.IsNullOrEmpty(destinationDir) && !Directory.Exists(destinationDir))
            {
                try
                {
                    Directory.CreateDirectory(destinationDir);
                }
                catch (UnauthorizedAccessException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
            }

            // Check if the given parameter is an existing file.
            if (File.Exists(input))
            {
                // If no output directory defined, use that
                // of the passed .ICO file.
                if (String.IsNullOrEmpty(destinationDir))
                {
                    destinationDir = Path.GetDirectoryName(input); // Output directory
                }

                // Extract the icons.
                ExtractIcon(input, destinationDir, fileFormat);
                return;
            }

            // If a diretory was passed, find and extract .ICO files of that directory.
            if (Directory.Exists(input))
            {
                // If no output directory defined, use that
                // of the passed .ICO file.
                if (String.IsNullOrEmpty(destinationDir))
                {
                    destinationDir = input; // Output directory
                }

                var lFiles = Directory.GetFiles(input, "*.ico");
                foreach (var lFilePath in lFiles)
                {
                    ExtractIcon(lFilePath, destinationDir, fileFormat);
                }
            }
        }

        private static FileFormat ParseFileFormat(string pFileFormat)
        {
            var fileFormat = pFileFormat.ToUpper();
            switch (fileFormat)
            {
                case "PNG" : return FileFormat.PNG;
                case "BMP" : return FileFormat.BMP;
                case "JPG" :
                case "JPEG": 
                    return FileFormat.JPEG;
                case "GIF" : return FileFormat.GIF;
                case "TIFF": return FileFormat.TIFF;
                case "WMP" : return FileFormat.WMP;
            }

            return FileFormat.Unknown;
        }

        private static void ExtractIcon(string pFilePath, string pOutputDirectory, FileFormat pFileFormat)
        {
            var lFileName = Path.GetFileName(pFilePath);
            var lIconStream = new FileStream(pFilePath, FileMode.Open);
            var lBitmapDecoder = new IconBitmapDecoder(lIconStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

            // Iterate over contained images and save them.
            foreach (var lBitmapFrame in lBitmapDecoder.Frames)
            {
                BitmapEncoder lEncoder = null;
                string fileExtension = String.Empty;

                // save file as PNG
                switch (pFileFormat)
                {
                    case FileFormat.PNG: 
                        lEncoder = new PngBitmapEncoder();
                        fileExtension = "png";
                        break;
                    case FileFormat.BMP: 
                        lEncoder = new BmpBitmapEncoder();
                        fileExtension = "bmp";
                        break;
                    case FileFormat.JPEG: 
                        lEncoder = new JpegBitmapEncoder();
                        fileExtension = "jpg";
                        break;
                    case FileFormat.GIF:
                        lEncoder = new GifBitmapEncoder();
                        fileExtension = "gif";
                        break;
                    case FileFormat.TIFF:
                        lEncoder = new TiffBitmapEncoder();
                        fileExtension = "tiff";
                        break;
                    case FileFormat.WMP:
                        lEncoder = new WmpBitmapEncoder();
                        fileExtension = "hdp";
                        break;
                }

                if (lEncoder == null) return;
                
                lEncoder.Frames.Add(lBitmapFrame);
                var lDimension = lBitmapFrame.PixelHeight;
                // Append backslash if required.
                if (!pOutputDirectory.EndsWith(@"\"))
                {
                    pOutputDirectory += @"\";
                }
                var lOutputFileName = String.Format("{0}_{1}x{1}.{2}", Path.GetFileNameWithoutExtension(lFileName), lDimension, fileExtension);
                var lOutputFilePath = pOutputDirectory + lOutputFileName;
                try
                {
                    using (var lStream = new FileStream(lOutputFilePath, FileMode.Create)) 
                    {
                        lEncoder.Save(lStream);
                    }
                }
                catch(UnauthorizedAccessException ex)
                {
                    Console.WriteLine(ex);
                    return;
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    return;
                }
                Console.WriteLine("Extracted file: {0}", lOutputFilePath);
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("  Usage:");
            Console.WriteLine();
            Console.WriteLine("     IcoConverter <ico_file|ico_directory>");
            Console.WriteLine("     IcoConverter -i <file|directory> [-d <dest_directory>] [-f <format>]");
            Console.WriteLine();
            Console.WriteLine("  Arguments:");
            Console.WriteLine();
            Console.WriteLine("     -i | --input <file|directory>  : .ICO file or directory with .ICO files.");
            Console.WriteLine("     -d | --destination <directory> : Destination directory for the images.");
            Console.WriteLine("     -f | --format <fileformat>     : File format of the extracted images.");
            Console.WriteLine("                                      Supported: PNG, BMP, JPEG, GIF, TIFF, WMP");
            Console.WriteLine();
            Console.WriteLine("  Samples:");
            Console.WriteLine();
            Console.WriteLine("     IcoConverter Foo.ico");
            Console.WriteLine("     IcoConverter C:\\MyIconCollection\\");
            Console.WriteLine("     IcoConverter -i Foo.ico -d C:\\DestinationDir\\ -f BMP");
        }
    }
}