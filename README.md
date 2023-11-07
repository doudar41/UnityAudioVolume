# UnityAudioVolume

I'll update video tutorial as soon as possible. For now this is an old one. 
[Video tutorial](https://youtu.be/A-vbIf3PII4?si=5kLcB9O0T-qnkk9I).

FMOD project preparations

Any event you’d like to use with Unity and scripts I made should have three parameters. LoudnessLocal, LowpassLocal and ReverbLocal. I use loudness to differ from audio volume volume. All these parameters should be in a range from 0 to 1. 
I recommend making one template, so you don’t need to set t all again every time. 
LoudnessLocal should automate Master Volume from silence to 0 db. LowpassLocal -  automates frequency of EQ in my case this is multiband EQ lowpass filter frequency. And finally ReverbLocal which would automate sending to the reverb bus in the mixer. 
You can change anything including names of parameters but you’ll need to adjust these names in scripts too. 
You can set any other parameters to use in Unity. I’ll tell you how to change them a bit later. 
It is good however to have them in range from 0 to 1 for consistency.


Unity notes

Let’s talk about limitations: 
You can’t have multiple rules for the same overlap region. Rules with the same audio volumes will be deleted in gameplay. 
Moving object logic is not implemented yet.
Portals will not work properly if audio volumes are overlapping

The idea behind audio volumes is very simple. 
Some sounds are linked to a collider and when a player enters this collider they start to play. Nothing revolutionary really. My task was to make some kind of dependencies between audio volumes.
So one thing I came up with was to make a separate script with a unique “rules” if a player inside two audio volumes at the same time. 

Possible usage cases:
Static changing of sound level of “outside” audio volume in smaller audio volume. Like entering a house without doors, leaving rain sound playing but reducing its level. 
Dynamic changing of outside sound depending on float variable. Rotation of a door can reduce or increase level of rain sound inside room 
Limiting sound propagation within a specific shape. Use colliders instead of raycasting to reduce water sound source behind rock formations, bridges etc.
Limit sound propagation of ambiences, especially in open areas. Like children's playgrounds near a house will partially be heard in the house due to the sphere shape of the sound source.

Other usage of audio volumes are portals. Audio volume marked as a portal applies to linked volumes static or dynamic effects if a player outside them. 
Possible use cases:
Players need to hear sounds inside the house being outside for example near the window or close to the door. But at the same time being inside to hear outside sounds. For example, to hear invisible enemy movement inside or outside the house. Or to imitate a realistic environment. 
It will be more usable when movable objects will be implemented. 

Bugs

I’m experimenting, that's why it’s possible to run into multiple bugs. The folder with scripts will be updated more often than unity packages, so if there are some issues take a look at the last commit, maybe the problem is fixed. 

**KNOWN ISSUES:**
1. Really wierd results of auto copy of mesh collider

Unity setup

Before importing “audiovolume.package” file it’s better to install FMOD and ProBuilder plugins to your project and also have a prepared FMOD project. Also you need: 
Have a layer called “SoundCollisions”. 
In project settings/Physics in layers matrix uncheck all layers which can overlap with SoundCollisions except the same one
Player character should have a rigidbody and game object with box collider, layer “SoundCollisions” and tag Player

You can download and launch audiovolume package while your project is opened, or you can download 
CollisionDetection.cs - place on gameobject with collision 
AVCrossfades.cs - you need one for scene
AudioVolume.cs - place on empty game object
CreateSmallerCube.cs - place on ProBuilder collider (or any custom Mesh Collider)
playerCollisionBox.cs (put on collision box on player)
SoundSource.cs - place on any game object

Other scripts and prefabs are optional

I’ll put together a couple of prefabs with all this logic so you can download and check  if they work. If you want a clean install this is the simplest one. 

To test if audio volume works you can:
Create an empty game object 
Put a it AVCrossfades script, call it “Crossfades”
Create an empty object
Put AudioVolume script on it
Drag and Drop Crossfades object to “Crossfades Rules” reference 
Create an empty object
Add a box collider component to it
Change layer to “SoundCollision”
Drag a CollisionDetection script on it
Add custom events to Enter and Exit unity events
Drag and Drop Audio Volume object in object field
In Enter context menu choose from the top menu function “OnEnterSimpleCollider”
In Exit context menu choose from the top menu function “OnExitSimpleCollider”
Create an empty object or use any object inside created collider 
Put SoundSource script on it choose any sound from your FMOD project
Put this object to a list of “Sound Sources” on audio volume object
Repeat steps from 4th to create second audio volume and sound source for it 
Make colliders bigger, place them the way so parts of them are overlapping
Open Crossfades game object
Add new Crossfade Rule to list 
Drop two volumes as volume references. 
Lower level of loudness and lowpass filters so you can hear difference 
Try it out

