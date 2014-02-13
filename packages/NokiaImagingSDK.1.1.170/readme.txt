
NuGet has successfully installed the SDK to your project !

Finalizing the installation
===========================
  - Some version of Visual studio may not find the references to the Nokia Imaging SDK that were added to your project by NuGet.  To fix things, simply close your project and reopen it. 
  - Make sure that your project doesn't have "Any CPU" as an "Active solution platform". You will find the instructions how to do this here: http://developer.nokia.com/resources/library/Lumia/nokia-imaging-sdk/adding-libraries-to-the-project.html
  

New Users
=========

If this is your first time with the Nokia Imaging SDK, welcome, we are glad to have you with us! To get you started off on the right foot, take a quick peek at our documentation :   
   http://developer.nokia.com/Resources/Library/Lumia/#!nokia-imaging-sdk.html

New in SDK 1.1.139
==================

1.1.139 is a bug fix release. Some of the issues fixed:

-  The BitmapRenderer's RenderAsync() threw an exception if the target Nokia.Graphics.Imaging.Bitmap had an odd number of lines (in NV12 mode), with a generic "wrong parameter" error message. The number of lines is now checked much earlier, in the Bitmap constructor, along with a host of other checks.  

-  On Windows 8.1, the size set on the WriteableBitmapRenderer would not affect the input size to some of the effects, potentially causing the effects to work on larger images than needed.

-  GradientImageSource didn't support offsets > 1.
 
-  Small fixes on the API documentation for the InteractiveForegroundSegmenter, HDREffect and CustomEffectBase.


New in SDK 1.1
==============

Windows 8.1 support

Since the Nokia Lumia family now includes a Windows RT 8.1 tablet, the Lumia 2520, we ported the library to the “big windows” platform. 


Foreground Segmenter

The new version of the SDK adds a filter API for picking the foreground/background from an image. Developers can use this component to create a UI where users tap, swipe, or point at the object boundaries and the API will figure out which objects belong to foreground and vice versa. This is often used for, for example, background swap or blurring the background with Bokeh.


Get Bokeh’d

A new effect, Lens Blur, also known as Bokeh, is a digital recreation of the familiar Bokeh effect so often used in professional photography. The Nokia Imaging SDK makes applying the new Bokeh effect or blending with mask quick and easy.


HDR effect

A new effect that can be applied to an image to create stunning and vibrant photorealistic color effects similar to HDR, or with more conservative settings, auto enhance the image colors.


Copyright (c) 2012-2014, Nokia
All rights reserved.





