# ScreepsCodeCommiter

Small c# tool to up and download ai code for screeps game with a WPF-Gui 

It can be used to quickly backup code from the Screeps Server to your local space and upload your new code to screeps.

You can also manage your branches, commit direct into a new branch or clone an existing branch.

Download: [Latest Release](https://github.com/Salmakis/ScreepsCodeCommiter/releases/latest)
If you want to build the Project by yourself, you will need the System.Json nuget package.

## How to Use:
Set the server settings in the Server Tab.
For the official server this would be 
address: https://screeps.com
token: your access token

Then select that server and hit the "Test" button to see if it works.

Info: The Token is being saved in clear readable text in your %appdata%/ScreepsCodeCommiter folder, I just wanted you to know that due to security reasons.

[![sct_server.png](https://s13.postimg.org/3pvb1ijqv/sct_server.png)](https://postimg.org/image/6jygeylwz/)

Then use the other tabs which are self-explanatory (i hope so).

## Code:

There are Several projects in the code
### ScreepsConnetion
It does all the Json and Get/Post stuff.
the ScreepsConnection library does all the Communication with screeps in async methods and gives out/takes c# Classes such as Room, CodeFile, etc.

It can be used with all kinds of C# Projects such as .Net4, .NetCore, Mono, Unity etc.

### ScreeepsCommiter
this is the project that this Site is About (actually)

### ScreeepsWpf
this was my first test project, will be removed 

### Projecttable

project | target          |Desc           | Type  |
---| --------------- |:-------------:| -----:|
ScreepsConnection| netStandart 2.0 | communication with screeps server|Lib |
ScreepsCodeCommiter| .net 4.6 wpf    |WPF tool for up/downloading    |tool|
ScreepsWpf| .net 4.6 wpf    |Wpf ugly tool for testing ScreepsConnection |debug tool |


