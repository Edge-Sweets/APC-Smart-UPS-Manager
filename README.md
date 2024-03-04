# APC-Smart-UPS-Manager

DEPRECATED.

This software is not still being updated as the software that comes with UPS devices now includes a way to gracefully shut down.


Software to control APC Smart UPS.  Gives status updates and allows you to change Battery Alarm, Graceful Shutdown, and Computer Shutdown Delays.  Replacement for APCs UPS Software.


Main Functions:

Poll UPS for status data including on battery, battery level, and voltages.
Shutdown Smart-UPS when computer is shut down manually
Shutdown Computer and UPS when UPS is on battery (after grace period)
Alert you when on battery power
Allow you to change:
    1. Battery alarm
    2. Grace Period for UPS Shutdown
    3. Grace Period for Computer Shutdown


UPS Power Management Software

The UPS Power Management Software is a custom piece of software used to integrate with the APC Smart UPS devices, specifically the SC type devices, but usable for a variety of them.  The main purpose of the software is to allow the configuration of the UPS device and allow for a graceful shutdown of both the UPS device and the computer in the event of computer shutdown or power loss, respectively.

Installing the UPS Power Management Software

The Power Management Software install will guide you through a wizard and install to an Edge-Sweets directory. An icon on the desktop will show up, and the software will automatically launch when your computer starts.

First Startup of the UPS Management Software

When you first launch the software, it will search your serial ports to determine if one of them has a connection to the UPS. Please be sure your UPS is plugged in, otherwise you will have to close the UPS Management Software and open it again to perform a new search.  There is no visual indication that a search is being done. If it finds a UPS, you will see the status information fill in on the configuration window.

Using the UPS Management Software

To use the UPS management software, first start it from the desktop icon.  Note: You will likely notice nothing pops up. This is because the software is designed to run in the background.  To view the configuration, check your icons in the bottom-right of your screen for a battery icon that matches the desktop icon.  Double-clicking the icon in the bottom-right will open the configuration screen.

UPS Configuration Screen

The UPS configuration screen contains all the feedback about your UPS, along with the ability to change a few of the settings.  Here is the list of status information shown:
1.	Model – This is the model of Smart-UPS that is currently connected
2.	Power Type – indicates whether you’re on line (normal) power or battery power
3.	Battery Level – indicates the level the battery is charged to
4.	Battery Alarm – tells you how often the alarm will sound to audibly notify you that you’re on battery power
5.	Battery Voltage – tells you what voltage the battery is currently running at.
6.	Input/Output voltage – indicates the voltage that is coming in to the UPS and the voltage that the UPS is outputting
7.	UPS Shutdown Delay – When the computer sends a signal to shut down, this is how long the UPS will delay before shutting down.
8.	Computer Shutdown Delay – When the computer detects the UPS is on battery power, this is how long the computer will delay before shutting down.
Below the status information are 3 options that you can change.
1.	Change Shutdown Delay – this will change the UPS Shutdown Delay
2.	Change Battery Alarm – this will change how often the battery audibly notifies you.
3.	Change Computer Shutdown Delay – This will change the amount of time the computer stays on after a loss of power is detected.
 

Shutting down the UPS Management Software

If for some reason you need to shut down the software, you can do so by right-clicking the icon in the bottom-right corner and selecting “Exit UPS Manager”.
 
Starting the Smart-UPS after a computer shutdown

If the computer is shutdown, a signal will be sent to the Smart-UPS to shut down as well.
This signal also tells the UPS to start back up when power comes back.
Please note: This means the order of events for shutting down a computer/UPS combination are:
1.	Shut down the computer
2.	Shut down power to the UPS within the allotted UPS Shutdown Delay Time Window.
If done in this order, the UPS will automatically start back up when you turn power back on. Otherwise you will have to turn the UPS on manually.

Starting the computer after a loss of power

If you lost power and the computer shut down, there is not currently a way to start the computer back up other than manually starting it yourself.
