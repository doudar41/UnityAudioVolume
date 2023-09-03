using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

[System.Serializable]
public struct AudioVolumeProperties
{
    public AudioVolume vol;
    public float volCrossVolume;
    
}

public class AudioVolume : MonoBehaviour
{
    bool enter1, enter2, exit1, exit2 = false;
    public bool inside = false;
    public UnityEvent enterAudioVolume;
    public UnityEvent exitAudioVolume;

    [Range(0,1)]
    public float thisVolumeInitLoudness = 1;
    public float thisVolumeCurrentVolume = 0;
    
    [SerializeField]
    private AnimationCurve FadeIn, FadeOut;

    public SoundSource[] emitters;
    public AudioVolumeProperties[] otherAudioVolumes;


    private void Start()
    {
        thisVolumeCurrentVolume = thisVolumeInitLoudness;
    }

    public void E1Enter() 
    {
        enter1 = true; 
        exit1 = false;
       //if (enter2) print("between colliders");
    }
    public void E2Enter() 
    {   enter2 = true;  
        exit2 = false;
       // if (enter1) print("between colliders");

    }
    public void E1Exit()
    {
        exit1 = true; 
        enter1 = false;
        if (!enter2)
        {
            if (exit2) { print("outside colliders"); exitAudioVolume.Invoke(); inside = false; exit2 = false; }
        }
    }
    
    public void E2Exit() { 
        exit2 = true; 
        enter2 = false;
        if (!enter1) { if (exit1) { print("inside colliders"); inside = true;  enterAudioVolume.Invoke(); exit1 = false; } }
    }
/*    private void Update()
    {
        ChangeVolumeOfChildrenSounds(thisVolumeCurrentVolume);
    }*/


    public void PlaySoundSources()
    {
        foreach (SoundSource x in emitters)
        {
            x.Play(this, FadeIn);
        }
    }

    public void StopSoundSources()
    {
        foreach (SoundSource x in emitters)
        {
            x.Stop(this, FadeOut);
        }
    }

    public void GradChangingLoudness(float volume)
    {
        if (emitters == null) return;
        bool isMore = true;
        isMore = CheckVolumeDifference(volume);
       foreach (SoundSource x in emitters)
        {
            if (isMore) x.ChangeVolumePlus(FadeIn,volume);
            else x.ChangeVolumeMinus(FadeOut, volume);
        }
    }

    public void OnPlayerInsideVolume()
    {
        if (otherAudioVolumes == null) return;
        foreach (AudioVolumeProperties x in otherAudioVolumes)
        {
            x.vol.thisVolumeCurrentVolume = x.volCrossVolume;
            x.vol.GradChangingLoudness(x.vol.thisVolumeCurrentVolume);
        }
    }
    public void OnPlayerOutsideVolume()
    {
        if (otherAudioVolumes == null) return;
        foreach (AudioVolumeProperties x in otherAudioVolumes)
        {
            x.vol.thisVolumeCurrentVolume = x.vol.thisVolumeInitLoudness;
            x.vol.GradChangingLoudness(x.vol.thisVolumeCurrentVolume);
        }
    }

    public void ApplyGlobalEffect(float effectAmount)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ReverbPrivate", effectAmount);
    }

    public void RemoveGlobalEffect()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("ReverbPrivate", 0);
    }

    bool CheckVolumeDifference(float input)
    {

        if (input > emitters[0].GetSourceVolume()) return true;
        else return false;
        
    }

    public void LowPass(float amount)
    {
        foreach (SoundSource x in emitters)
        {
            x.ChangeLFP(amount);
        }
    }

}
