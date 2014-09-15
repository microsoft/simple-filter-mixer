Simple Filter Mixer
===================

Simple Filter Mixer is an example application for Windows Phone demonstrating
the use of Imaging SDK APIs, combining multiple filters together and
demonstrating each of the available filter in the SDK. This sample also
demonstrates on-demand creation of the filters using reflection.

This example application is hosted in GitHub:
https://github.com/nokia-developer/simple-filter-mixer/

Developed with Microsoft Visual Studio 2013. Compatible with Windows Phone 8.1
and Windows 8.1. However, the user interface is optimised for the phone only and
only the phone version is fully tested. Tested to work on: Nokia Lumia 930,
Nokia Lumia 1020 and Nokia Lumia 1520.

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


Important classes
-----------------

| Class | Description |
| ----- | ----------- |
| [FiltersPage](/simple-filter-mixer/simple-filter-mixer.Shared/FiltersPage.xaml.cs) | Displays a grid view of all the available filters. On this page filters can be selected and you can open the settings of any filter. |
| [Imaging](/simple-filter-mixer/simple-filter-mixer.Shared/Imaging.cs) | Creates the filters (based on the definitions in [FilterDefinitions.cs](/simple-filter-mixer/simple-filter-mixer.Shared/DataModel/FilterDefinitions.cs) and renders the image using the filters. Contains implementation for applying modified filter settings. |
| [SettingsPage](/simple-filter-mixer/simple-filter-mixer.Shared/SettingsPage.xaml.cs) | Retrieves the properties of any filter using reflection. Creates the UI controls for modifying the filter property values and translates the values in controls back to correct property value types, which are then applied by `Imaging` class. |


Known issues
------------

* No implementation of UI controls for modifying the following filter properties:
 * `Nokia.Graphics.Imaging.IImageSource` (used e.g. by `BlendFilter`)
 * `Nokia.Graphics.Imaging.Curve` (used by `CurvesFilter`)
 * List of regions for `WarpFilter`
* The image is re-rendered when filter settings are modified even if that filter
  was not selected


License
-------

See the license text file delivered with this project:
https://github.com/nokia-developer/simple-filter-mixer/blob/master/License.txt


Version history
---------------

* 1.1: A ton of fixes.
* 1.0: The first public release of Simple Filter Mixer.
