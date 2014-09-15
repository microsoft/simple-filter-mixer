Simple Filter Mixer
===================

Simple Filter Mixer is an example application for Windows Phone demonstrating the use of Nokia Imaging SDK APIs, combining multiple filters together and demonstrating each of the available filter in the SDK. This sample also demonstrates on-demand creation of the filters using reflection.

This example application is hosted in GitHub:
https://github.com/nokia-developer/simple-filter-mixer/

Developed with Microsoft Visual Studio 2013.

Compatible with Windows Phone 8.1 and Windows 8.1. However, the user interface
is optimised for the phone only and only the phone version is fully tested.

Tested to work on: Nokia Lumia 930, Nokia Lumia 1020 and Nokia Lumia 1520.

![Main page](/doc/screenshots/MainPageFiltersAppliedWPSmall.png?raw=true "Main page")&nbsp;
![Filter selection page](/doc/screenshots/FiltersPageWPSmall.png?raw=true "Filter selection page")&nbsp;
![Settings page](/doc/screenshots/SettingsPageWP2Small.png?raw=true "Filter selection page")&nbsp;

Instructions
------------

Make sure you have the following installed:

 * Windows 8.1
 * Visual Studio Express 2013 for Windows
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
https://github.com/nokia-developer/simple-filter-mixer/blob/master/License.txt


Downloads
---------

| Project | Release | Download |
| ------- | --------| -------- |
| Simple Filter Mixer | v1.0 | [simple-filter-mixer-1.0.zip](https://github.com/nokia-developer/simple-filter-mixer/archive/v1.0.zip) |


Version history
---------------

 * 1.0.0.0: First public release of Simple Filter Mixer
