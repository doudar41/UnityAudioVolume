# UnityAudioVolume

/b FMOD set up. /b

In FMOD projects you need any event you want to use with audio volumes that have two parameters: LoudnessLocal and LowpassLocal. You can set them up like this. 

For Low pass you can use any EQ or Resonance Audio plugin
In my case its Multiband EQ

Every parameter in FMOD has a velocity and Seek Speed Parameters you can use it to soften transitions between audio volumes and crossfades. I’m planning to make it in Unity but for the moment it’s still like this. 

Unity set up simple

Before starting it’s better to install FMOD and ProBuilder plugins and also have a prepared FMOD project. You need to have a layer called “SoundCollisions”. And in project settings/Physics in layers matrix uncheck all layers which can overlap with SoundCollisions except the same one. 

I made a unity package with prefabs which can be used in a project. 

You need:
Crossfade rule, 
at least two prefabs of audio volumes simple or mesh, 
sound collision box attached to player gameobject

You need to populate soundsources inside volumes prefabs with events from your project. 
Hopefully it’ll work if not this is a long explanation of what is what.
