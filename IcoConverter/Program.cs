/*

    IcoConverter - Extracts images from .ICO files
    Copyright (C) 2013  Daniel Lemke <lemked@web.de>

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

namespace IcoToPng
{
    class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine("IcoConverter - Extracts images from .ICO files");
            Console.WriteLine("Copyright (C) 2013  Daniel Lemke <lemked@web.de>");
            Console.WriteLine();

            if (args.Length.Equals(0) || args[0].Equals("/?") || args[0].Equals("-h"))
            {
                ShowUsage();
                return;
            }

            var lFileOrDirectory = args[0]; // .ICO filename or directory

            string lOutputDir = String.Empty;
            if (args.Length > 1 && !String.IsNullOrEmpty(args[1]))
            {
                lOutputDir = args[1];
            }

            if (!String.IsNullOrEmpty(lOutputDir) && !Directory.Exists(lOutputDir))
            {
                try
                {
                    Directory.CreateDirectory(lOutputDir);
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
            if (File.Exists(lFileOrDirectory))
            {
                // If no output directory defined, use that
                // of the passed .ICO file.
                if (String.IsNullOrEmpty(lOutputDir))
                {
                    lOutputDir = Path.GetDirectoryName(lFileOrDirectory); // Output directory
                }

                // Extract the icons.
                ExtractIcon(lFileOrDirectory, lOutputDir);
                return;
            }

            // If a diretory was passed, find and extract .ICO files of that directory.
            if (Directory.Exists(lFileOrDirectory))
            {
                // If no output directory defined, use that
                // of the passed .ICO file.
                if (String.IsNullOrEmpty(lOutputDir))
                {
                    lOutputDir = lFileOrDirectory; // Output directory
                }

                var lFiles = Directory.GetFiles(lFileOrDirectory, "*.ico");
                foreach (var lFilePath in lFiles)
                {
                    ExtractIcon(lFilePath, lOutputDir);
                }
            }
        }

        private static void ExtractIcon(string pFilePath, string pOutputDirectory)
        {
            var lFileName = Path.GetFileName(pFilePath);
            var lIconStream = new FileStream(pFilePath, FileMode.Open);
            var lBitmapDecoder = new IconBitmapDecoder(lIconStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);

            // Iterate over contained images and save them.
            foreach (var lBitmapFrame in lBitmapDecoder.Frames)
            {
                // save file as PNG
                var lPngBitmapEncoder = new PngBitmapEncoder();
                lPngBitmapEncoder.Frames.Add(lBitmapFrame);
                var lDimension = lBitmapFrame.PixelHeight;
                // Append backslash if required.
                if (!pOutputDirectory.EndsWith(@"\"))
                {
                    pOutputDirectory += @"\";
                }
                var lOutputFileName = String.Format("{0}_{1}x{1}.png", Path.GetFileNameWithoutExtension(lFileName), lDimension);
                var lOutputFilePath = pOutputDirectory + lOutputFileName;
                try
                {
                    using (var lStream = new FileStream(lOutputFilePath, FileMode.Create)) 
                    {
                        lPngBitmapEncoder.Save(lStream);
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
            Console.Write("IcoConverter <ICO file or directory> [destination directory]");
        }
    }
}