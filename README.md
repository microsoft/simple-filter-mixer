Simple Filter Sample
====================

Simple Filter Sample is an example application for Windows Phone demonstrating the use of Nokia Imaging SDK APIs, combining multiple filters together and demonstrating each of the available filter in the SDK.

This example application is hosted in GitHub:
https://github.com/nokia-developer/simple-filter-sample/

Developed with Microsoft Visual Studio 2013.

Compatible with:

 * Windows Phone 8

Tested to work on:

 * Nokia Lumia 1020
 

Instructions
------------

Make sure you have the following installed:

 * Windows 8
 * Visual Studio Express 2012 for Windows Phone
 * Nuget 2.7 or later

To build and run the sample in emulator

1. Open the SLN file:
   File > Open Project, select the solution (.sln postfix) file
2. Select the target 'Emulator' and platform 'x86'.
3. Press F5 to build the project and run it.


If the project does not compile on the first attempt it's possible that you
did not have the required packages yet. With Nuget 2.7 or later the missing
packages are fetched automatically when build process is invoked, so try
building again. If some packages cannot be found there should be an
error stating this in the Output panel in Visual Studio Express.

For more information on deploying and testing applications see:
http://msdn.microsoft.com/library/windowsphone/develop/ff402565(v=vs.105).aspx


About the implementation
------------------------

| Folder | Description |
| ------ | ----------- |
| / | Contains the project file, the license information and this file (README.md) |
| ImagingSDKSamples | Root folder for the implementation files.  |
| ImagingSDKSamples/Assets | Graphic assets like icons and tiles. |
| ImagingSDKSamples/Resources | Localized resources. |
| ImagingSDKSamples/Properties | Application property files. |
| ImagingSDKSamples/Toolkit.Content | Graphic assets. |

Important classes:

| Class | Description |
| ----- | ----------- |
| MainPage | Contains all the logic. |


Known issues
------------

 * App has only the bare minimum code to demonstate filters, and all marketplace release requirements are not implemented


License
-------

See the license text file delivered with this project:
https://github.com/nokia-developer/lens-blur/blob/master/License.txt


Downloads
---------

| Project | Release | Download |
| ------- | --------| -------- |
| Simple Filter Sample | v1.0 | [simple-filter-sample-1.0.zip](https://github.com/nokia-developer/simple-filter-sample/archive/v1.0.zip) |


Version history
---------------

 * 1.0.0.0: First public release of Simple Filter Sample
