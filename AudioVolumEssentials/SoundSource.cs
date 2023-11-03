using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class SoundSource : MonoBehaviour
{
    [SerializeField]
    [Range(0,1)]
    float loudnessDefault = 1; 
    
    [SerializeField]
    [Range(0, 1)]
    float lowPassFreqDefault= 1;

    public bool Movable = false; //Apply reverb to moving sound sources
    public bool PlayOnStart = false;
    Transform objectAttachTo;

    public EventReference EventName;
    FMOD.Studio.EventInstance soundInstance;

    private void Awake()
    {
        soundInstance = FMODUnity.RuntimeManager.CreateInstance(EventName);
        objectAttachTo = gameObject.transform;
        if (!soundInstance.isValid())
        {
            print("No valid event");
        }
        
       if (PlayOnStart) Play();
    }

    public bool checkedIfPlaying()
    {
        soundInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        if (state == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            return true;
        }
        else
            return false;
    }


    private void Update()
    {
        soundInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }


    public void Play()
    {
        soundInstance.start();
        ChangeLoudness(loudnessDefault);
        ChangeLowPass(lowPassFreqDefault);
    }

    public void Stop()
    {
        soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void ChangeGameObject(Transform objectOnScene)
    {
        objectAttachTo = objectOnScene;
    }


    public void ChangeLoudness(float amount)
    {
        //Should be local parameter on FMOD event called "LoudnessLocal"
        soundInstance.setParameterByName("LoudnessLocal", amount);
    }

    public void ChangeLowPass(float amount)
    {
        soundInstance.setParameterByName("LowpassLocal", amount);
    }

    public void ChangeReverb(float amount)
    {
        soundInstance.setParameterByName("ReverbLocal", amount);
    }

    public void ChangeCustomEffect(string parameterName, float amount)
    {
        soundInstance.setParameterByName(parameterName, amount);
    }

    public void ResetToDefault()
    {
        soundInstance.setParameterByName("LoudnessLocal", loudnessDefault);
        soundInstance.setParameterByName("LowpassLocal", lowPassFreqDefault);
    }

    private void OnDestroy()
    {
        soundInstance.setUserData(System.IntPtr.Zero);
        soundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        soundInstance.release();
    }

}
