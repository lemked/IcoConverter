IcoConverter - Extracts images from .ICO files

Copyright (C) 2013 - Daniel Lemke <lemked@web.de>

This application can be used to extract images from a .ICO 
file or a directory that contains .ICO files and save them
into a image format like .PNG.

  Usage:

     IcoConverter <ico_file|ico_directory>
     IcoConverter -i <file|directory> [-d <dest_directory>] [-f <format>]

  Arguments:

     -i | --input <file|directory>  : .ICO file or directory with .ICO files.
     -d | --destination <directory> : Destination directory for the images.
     -f | --format <fileformat>     : File format of the extracted images.
                                      Supported: PNG, BMP, JPEG, GIF, TIFF, WMP

  Samples:

     IcoConverter Foo.ico
     IcoConverter C:\MyIconCollection\
     IcoConverter -i Foo.ico -d C:\DestinationDir\ -f BMP