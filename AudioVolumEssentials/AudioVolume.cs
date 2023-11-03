using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum soundSourceData
{
    loundness,
    lowpassfreq,
    reverbLevel
}

public class AudioVolume : MonoBehaviour
{
    
    [HideInInspector]
    public int avName;

    [Header("Place your functions here")]
    [Header("`````````````````````````")]
    public UnityEvent enterAudioVolume;
    public UnityEvent exitAudioVolume;

    [Range(0,1)]
    public float soundSourcesReverbLevel = 0;
    public bool noVolumeFades = false;
    public float fadeSpeed = 1;

    bool inside = false;

    //If player inside audio volume this script checking for movable sounds inside it and fill this list
    List<SoundSource> movableSoundSourcesInside = new List<SoundSource>();

    //Send signal to AVCrossfades class on enter and exit volume with this volume ID
    public delegate void TransferName (int myint); 
    TransferName enterAudioVolumeForCrossfades;
    TransferName exitAudioVolumeForCrossfades;

    //Array of sound sources attached to to audio volume
    public SoundSource[] soundSources;
    
    // Bools using for mesh colliders logic
    bool enter1, enter2, exit1, exit2 = false; 

    // Used for referencing events and player character
    public AVCrossfades crossfadesRules;

    private void Awake()
    {
        avName = GetHashCode(); 
        enterAudioVolumeForCrossfades = crossfadesRules.AddAV;
        exitAudioVolumeForCrossfades = crossfadesRules.RemoveAV;
    }

    public void AttachEnterAction(UnityAction act)
    {
        enterAudioVolume.AddListener(act);
    }
    public void AttachExitAction(UnityAction act)
    {
        exitAudioVolume.AddListener(act);
    }

    private void Start()
    {
        ChangeSoundSources(soundSourceData.reverbLevel, soundSourcesReverbLevel);
    }

    public void StartOnPlayerInside(Collider collider)
    {
        inside = true;
        enterAudioVolume.Invoke();
        enterAudioVolumeForCrossfades(avName);
        //print("start player function " + collider.transform.parent.name);
    }

    /// <summary>
    /// This is logic using for mesh colliders to send events on enter and exit
    /// </summary>

    public void CollisionOneEnter(Collider soundSourceEntered)
    {
        if (soundSourceEntered.tag != "Player") return;
        enter1 = true;
        exit1 = false;
    }
    public void CollisionTwoEnter(Collider soundSourceEntered)
    {
        if (soundSourceEntered.tag != "Player") return;
        enter2 = true;
        exit2 = false;
    }
    public void CollisionOneExit(Collider soundSourceExited)
    {
        if (soundSourceExited.tag != "Player") return;
        exit1 = true;
        enter1 = false;
        if (!enter2)
        {
            if (exit2) {  
                if (inside)
                {
                    OnExitSimpleCollider(soundSourceExited);
                }
                
                inside = false; exit2 = false;
                //print("exit volume"+this.name);
            }
        }
    }

    public void CollisionTwoExit(Collider soundSourceExited)
    {
        if (soundSourceExited.tag != "Player") return;
        exit2 = true;
        enter2 = false;
        if (!enter1) { if (exit1) 
            { if (!inside)
                {
                    OnEnterSimpleCollider(soundSourceExited);
                }
            inside = true; exit1 = false; 
            } 
        }
    }

    /// <summary>
    /// This is logic that used for simple colliders and as functions when player inside complex collider
    /// </summary>

    public void OnEnterSimpleCollider(Collider soundSourceEntered)
    {
        if (soundSourceEntered.tag == "Player")
        {
            enterAudioVolume.Invoke();
            enterAudioVolumeForCrossfades(avName);
            print("on enter function on volume - " + name);
        }
    }

    public void OnExitSimpleCollider(Collider soundSourceExited)
    {
        if (soundSourceExited.tag == "Player")
        {
            exitAudioVolume.Invoke();
            exitAudioVolumeForCrossfades(avName);
            print("player exits ");

        }
    }

    public void ChangeSoundSources(soundSourceData dataToChange, float amount)
    {
        switch (dataToChange)
        {
            case soundSourceData.loundness:
                foreach (SoundSource sound in soundSources)
                {
                    if (noVolumeFades) sound.ChangeLoudness(amount);
                    else
                    {
                        //StartCoroutine(FollowCurveFadeIn(sound))
                    }
                }
                break;
            case soundSourceData.lowpassfreq:
                foreach (SoundSource sound in soundSources)
                {
                    sound.ChangeLowPass(amount);
                }
                break;
            case soundSourceData.reverbLevel:
                foreach (SoundSource sound in soundSources)
                {
                    sound.ChangeReverb(amount);
                }
                break;
        }
    }

    public void ResetSoundsToDefault()
    {
        foreach (SoundSource sound in soundSources)
        {
            sound.ResetToDefault();
        }
    }


/* * 
 * This functions allows to use curves for fade in and out FMOD events parameters
 **/
    IEnumerator FollowCurveFadeIn(float startPoint, float targetPoint, AnimationCurve curve, soundSourceData variableToChange)
    {
        float tempValue = startPoint;
        do
        {
            curve.Evaluate(tempValue);
            tempValue += Time.deltaTime;
            ChangeSoundSources(variableToChange, tempValue);
            yield return new WaitForSeconds(fadeSpeed);
        } while (tempValue > targetPoint);

        yield return null;
    }

    IEnumerator FollowCurveFadeOut(float startPoint, float targetPoint, AnimationCurve curve, soundSourceData variableToChange)
    {
        float tempValue = startPoint;
        do
        {
            curve.Evaluate(tempValue);
            tempValue -= Time.deltaTime;
            ChangeSoundSources(variableToChange, tempValue);
            yield return new WaitForSeconds(fadeSpeed);
        } while (tempValue < targetPoint);

        yield return null;
    }

    private void OnDestroy()
    {
        enterAudioVolume.RemoveAllListeners();
        exitAudioVolume.RemoveAllListeners();
        
    }

    public float ReturnSizeOfAudioVolume()
    {
        Collider col = GetComponentInChildren<Collider>();
        return col.bounds.size.sqrMagnitude;
    }

    public void Giveback(List<SoundSource> listSoundSources)
    {
        movableSoundSourcesInside = listSoundSources;
    }

}
