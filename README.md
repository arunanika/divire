<h1 align="center">
  <img src="https://user-images.githubusercontent.com/63274471/85964833-ea76c600-b9f5-11ea-849b-9726bc56fa5a.jpg" alt="divire">
</h1>

# About
divire is a utility that automatically paints line art image files and converts them to layer formats. divire can run on Windows.

<h1 align="center">
  <img src="https://user-images.githubusercontent.com/63274471/85966176-efd60f80-b9f9-11ea-941b-f8e83a546ce4.jpg" alt="divire_demo">
</h1>

# Requirements

- .NET Framework 4.8

# Build
1. Install Visual Studio 2017 with .NET desktop development and Desktop development with C++. 



2. Download and install OpenCV (Use a 4.0.0 version) at http://opencv.org/releases.html.



3. Create that OpenCV solution for "Visual Studio 15 2017 Win64" with [CMake](https://cmake.org/).

   Use the following option : -D BUILD_SHARED_LIBS=OFF
   
   
   
4. Open the created OpenCV solution (OpenCV.sln) and build the "INSTALL" project (in the "CMakeTargets" Folder).

   Static library and header files will be generated in the "install" folder in the same hierarchy as the OpenCV solution file.
   
   
   
5. In the root folder of the divire project, create the following folder : \packages\opencv\install


6. Copy the "include" and "x64" folders in the "install" folder created in **step 4** to the "install" folder created in **step 5**.


7. Build "ColoringTrial.sln" in the divire project.


# Documentation

You can find the full documentation of divire at [arunanika.com](https://arunanika.com/).

# Author
[Aru Nanika](https://arunanika.com/)

# License

divire is licensed under [MIT](https://raw.githubusercontent.com/aocattleya/Ramen-Timer/master/LICENSE).

Copyright (C) 2020 Aru Nanika
