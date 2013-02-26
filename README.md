UAV-NET
=======

UAV.Net Framework

The last 2 years if been working on a robotics framework for c# / .net.

I am using it for my UAV project. It uses a Pandaboard running ubuntu server linux, a razor imu and the mini maestro as the output.
I think it is an interessting idea to develop an UAV Controller using a modern CPU and OS instead of an embedded CPU. 

Features:

Mini Maestro for 

  PWM output support (12 ch optimization to write multiple channels at once) ~ < 1ms per write
  Analog Input 
  Digital IO   
Pwm sum signal from rc reciever via pwm to usb adapter and fast joystick input (windows / linux) 
Sparkfun Razor IMU with custom firmware for 100 hz output
GPS NMEA Parser (tested with Venus gps)
PID library 
Single step debugging on device via mono soft debugger or debugging locally on windows (All Hardware can be accessed via usb on windows
Event driven communication and programming:
      all UAV Parameters e.g. PID Gains can be changed in flight
Command framework
Communication via WLAN or 3G 
	TCP & UDP support
Loop is working well at 100 HZ
Framework is optimized for low garbage collection overhead.
Logging with Log4Net

GroundControl:

Winforms Application 
	Dockable Pads
	Layout Manager
	UAV Session Manager
OpenGL Visualisation of UAV Attitude
UAV Parameter Visual Studio style Property editor
UAv Parameter slider editor and visualisatioon
Graphing with Zedgraph
Mission Planer including Map and Waypointeditor


It is much easier to access hardware, if it is already supported by linux. For example i was using the PS3 eye toy cam to get a low latency video stream over wifi to the pc 
Latency was below 150 ms.

It would also be quite easy to access a kinect device using the .net libs available, but i guess that would make more sense on a rover. 


i've released the source code under the GPLv2 in the hope that it will be usefull to others, and to speed up development. Remember this is more a framework, than an autopilot. But the plan is that it will become one someday :-)


You can find it in my SVN repository at:
http://siegelrnd.at/svn/uav/Software

online documentation can be accessed at:
http://siegelrnd.at/uav/html


Peter Siegel
siegelpeter2009(at)gmail.com

3rd Party Libaries used:

Pololu Mini Maestro .net Wrapper (with modifications to write multiple targets at once)
SharpGPS
PIDLibary http://www.codeproject.com/Articles/49548/Industrial-NET-PID-Controllers with modifications to work directly on UAV Parameters
dockpanelsuite http://sourceforge.net/projects/dockpanelsuite/
MonoLibUSB
GMAP.NET
Zedgraph
Gavaghan.Geodesy
Log4Net
OpenGL / FreeGLUT / SDL









