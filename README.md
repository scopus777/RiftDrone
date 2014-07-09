RiftDrone
============

This unity project combines the Oculus Rift with the AR.Drone 2.0. A small cockpit with a big screen at the front was created. The screen displays the view of the drone. The user can use the oculus rift to orientate himself in the cockpit. If he looks down, there is a smaller screen where he can see the view of the bottom drone camera. The drone is controlled with a xbox 360 controller.
Because of legal issues this project does not contain the functionalties for the oculus rift. But it can be simple integrated manually. Read the chapter Installation für further instructions.
This project is made for Windows and only tested under Windows 7 64-Bit.

For a small impression you can watch these videos: 
https://www.youtube.com/watch?v=3WmQ1_8ytaM
https://www.youtube.com/watch?v=b68LKfNzn9c

### Requirements:
* [Ar.Drone 2.0](http://ardrone2.parrot.com)
* [Oculus Rift](http://www.oculusvr.com)
* [Xbox 360 Controller](http://www.xbox.com/de-DE/Xbox360/Accessories/Controllers)
* [Unity 3D Pro](https://unity3d.com/) (Used Version: 4.3.4f1)

### Used libraries:
* [Ruslan-B/AR.Drone C# Framework](https://github.com/Ruslan-B/AR.Drone)
	- A C# Framework to control the AR.Drone. Originally this framework was made for .NET 4.0. I had to make same changes to make it possible to run the framework with .NET 3.5, because Unity only supports .NET up to the version 3.5. All needed files are contained in the script folder. More details about the changes can be found in the chapter “Changes on the AR.Drone C# Framework”.
* [Task Parallel Library for .NET 3.5](http://www.nuget.org/packages/TaskParallelLibrary/1.0.2856)
	- The AR.Drone framework uses some threading class which are not available in .NET 3.5. That’s why I had to add this library.
* [Ruslan-B/FFmpeg.AutoGen](https://github.com/Ruslan-B/FFmpeg.AutoGen)
	- A C# Wrapper for FFmpeg. This is needed to decode the video packets of the AR.Drone. The DLLs of this problem causing a little bit of trouble. If you simple put them into the directory "Assets\Plugins\x86" they are not found in the game. So you need to put them into the unity editor directory if you want to test the game in the editor. After you building, you have to put the DLLs in the directory of the exe to work correctly. You can find the FFmpeg DLLs in "Assets\Plugins\x86\FFmpeg".
* [Oculus Rift Plugin for Unity](http://paddytherabbit.com/unity3d-oculus-rift-plugin-setup)
	- This Plugin is needed to integrate the oculus rift. If you follow the link you will find a setup guide for the plugin.
* [XInputDotNet](https://github.com/speps/XInputDotNet) 
	- This library is used to manage a connected xbox controller.
* [Cockpit of the FA-38 3D-Model](http://tf3dm.com/3d-model/black-ops-2-fa-38-70010.html)
	- Thanks to ysup12
	
### Installation:
This project does not include every Library needed to execute the application. Therefore you have to follow the following instructions.

1. Download the [Task Parallel Library for .NET 3.5](http://www.nuget.org/packages/TaskParallelLibrary/1.0.2856) and put “System.Threading.dll” into the folder “Assets\Plugins\x86” 
2. Download [XInputDotNet](https://github.com/speps/XInputDotNet) for Unity and put “XInputDotNetPure.dll” and “XInputInterface.dll” into the folder “Assets\Plugins\x86”
3. Put the files situated in the directory “Assets\Plugins\x86\FFmpeg” into the directory of unity editor (e.g C:\Program Files (x86)\Unity\Editor). Otherwise unity is not able to find these secondary libraries. If you compile the application, then those files have also to be placed in the directory where the exe file of the application is situated.  
4. (Optional) – Delete the Camera GameObject and follow [these](http://paddytherabbit.com/unity3d-oculus-rift-plugin-setup) instructions to integrate the oculus rift. Place the OVRCameraController gameobject at a suited position (where the head of the pilot would be most likely).

### Changes on the AR.Drone C#-Framework
* StopWatch.Restart() replaced by StopWatch.Reset() and StopWatch.Start() in:
	- AR.Drone.Client\Command\CommandSender.cs
	- AR.Drone.Client\Configuration\ConfigurationAcquisition.cs
	- AR.Drone.Client\Navigation\NavdataAcquisition.cs
	
* added AR.Drone.Client\EnumHelper.cs ([source](http://stackoverflow.com/questions/15017151/implementation-of-enum-tryparse-in-net-3-5)) containing TryParse method. Resulting Changes:
	- replaced Enum.TryParse(parts[0], out type) in AR.Drone.Client\Configuration\FlightAnimation.cs by EnumHelper.TryParseEnum(parts[0], out type)
	- replaced Enum.TryParse(parts[0], out type) in AR.Drone.Client\Configuration\LedAnimation.cs by EnumHelper.TryParseEnum(parts[0], out type)
	- replaced Enum.TryParse(parts[0], out type) in AR.Drone.Client\Configuration\UserboxCommand.cs by EnumHelper.TryParseEnum(parts[0], out type)
	
- added AR.Drone.Data\EnumExtensions.cs containing HasFlag method ([source](http://www.sambeauvois.be/blog/2011/08/enum-hasflag-method-extension-for-4-0-framework/))

### License

Copyright 2014 Matthias Weise matthiasweise@online.de

GNU Lesser General Public License (LGPL) version 3 or later.  
http://www.gnu.org/licenses/lgpl.html

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.