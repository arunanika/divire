<h1 align="center">
  <img src="https://user-images.githubusercontent.com/63274471/85964833-ea76c600-b9f5-11ea-849b-9726bc56fa5a.jpg" alt="divire">
</h1>

# About
divire is a utility that automatically paints line art image files and converts them to layered image formats. divire can run on Windows.

<h1 align="center">
  <img src="https://user-images.githubusercontent.com/63274471/85966176-efd60f80-b9f9-11ea-941b-f8e83a546ce4.jpg" alt="divire_demo">
</h1>


# Download

The latest version of divire is available [here](https://github.com/arunanika/divire/releases).

# Requirements

- .NET Framework 4.8

# Build
1. Install Visual Studio 2017 with .NET desktop development and Desktop development with C++. 



2. Download OpenCV (Use a 4.3.0 version) sources at http://opencv.org/releases.html.



3. Create an OpenCV solution for "Visual Studio 15 2017 Win64" using [CMake](https://cmake.org/) with the value of the "BUILD_SHARED_LIBS" option set to OFF.
  
   
   
4. Open the created OpenCV solution (OpenCV.sln) and build the "INSTALL" project (in the "CMakeTargets" Folder).

   Static-link libraries and header files will be generated in the "install" folder in the same hierarchy as the OpenCV solution file.
   
   
   
5. In the root folder of the **divire** repository, create the following folder : \packages\opencv\install


6. Copy the "include" and "x64" folders in the "install" folder created in **step 4** to the "install" folder created in **step 5**.


7. Open "ColoringTrial.sln" in Visual Studio, and build with the target platform as "x64".


# Documentation

You can find the full documentation of divire at [arunanika.com](https://arunanika.com/).

# Author
[Aru Nanika](https://arunanika.com/)

# License

divire is licensed under [MIT](https://raw.githubusercontent.com/aocattleya/Ramen-Timer/master/LICENSE).

Copyright (C) 2020 Aru Nanika
