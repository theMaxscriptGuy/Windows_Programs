This software is created using DOTNET 4.5

How it works?

The softwares takes input from the user and processes images using another software and exports the Output.

1) Input Folder takes in a folder path which contains .png files. Then it does a search in the folder (Top Folder only or All Directories which is recursive) for .png files.

2) Dependencies takes the path to the "Build" folder.

3) Output Folder takes the path to a folder where the output files are to be created.

4) File Name is the name of the zip file which is to be created.

Process:

The software takes the input deatails and makes a JSON out of it. This JSON is saved in the Build folder.
Path: Build\Packer_Data\StreamingAssets\files.txt

The JSON comprises of the Output Path and the File name and the image files locations.

Then the application starts another Process which is an exe. It is kept here:
Build\Packer.exe

This exe takes the JSON file and reads it.

Packer runs in the background and creates the necessary files.
ios - PVRTC
android - ASTC
pc - DXT

The application Packer then quits and the "Texture Packer" application makes the zip file.

The process was made such as I wanted to show how we can make Unity exe and utilize them for our work in c#. How we can use our resources to the fullest and avoid outside libraries.

Thanks,
Videep