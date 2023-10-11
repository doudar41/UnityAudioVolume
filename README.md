# UnityAudioVolume

[Video tutorial](https://youtu.be/A-vbIf3PII4?si=5kLcB9O0T-qnkk9I).

**KNOWN ISSUES:**
1. If player inside volume at the game start no sound playing. Add Start with Player Inside bool to Audio volume.
2. Volume names are hashcodes of game objects, can change during a game should be reinitialized
3. Two mesh colliders should have a gap between them less then trigger on player. if smaller collider is outside bigger one different weird behavior expected.  


FMOD set up.

In FMOD projects you need any event you want to use with audio volumes that have two parameters: LoudnessLocal and LowpassLocal. 
For Low pass you can use any EQ or Resonance Audio plugin in my case its Multiband EQ.

Every parameter in FMOD has a velocity and Seek Speed Parameters you can use it to soften transitions between audio volumes and crossfades. I’m planning to make it in Unity but for the moment it’s still like this. 

Unity set up simple.

Before starting it’s better to install FMOD and ProBuilder plugins and also have a prepared FMOD project. 
You need:
to have a layer called “SoundCollisions”. 
In project settings/Physics in layers matrix uncheck all layers which can overlap with SoundCollisions except the same one. 

I made a unity package with prefabs which can be used in a project. 

To test you need use prefabs from AudioVolumEssentials folder:
Crossfade rule
at least two prefabs of audio volumes simple or mesh, 
Sound Collision box attached to player gameobject
Populate soundsources inside volumes prefabs with events from your project. 

Hopefully it’ll work 
