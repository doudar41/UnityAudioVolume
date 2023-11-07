using System.Collections;
using System.Linq;
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
    
    //[HideInInspector]
    public int avName;

    [Header("Place your functions here")]
    [Header("`````````````````````````")]
    public UnityEvent enterAudioVolume;
    public UnityEvent exitAudioVolume;

    public bool portal = false;
    public bool zeroLoudnessAfterExit = true;

    [Range(0,1)]
    public float soundSourcesReverbLevel = 0;

    //Send signal to AVCrossfades class on enter and exit volume with this volume ID
    public delegate void TransferName (int myint); 
    TransferName enterAudioVolumeForCrossfades;
    TransferName exitAudioVolumeForCrossfades;

    //Array of sound sources attached to to audio volume
    public List<SoundSource> soundSources = new List<SoundSource>();
    
    // Used for referencing events and player character
    public AVCrossfades crossfadesRules;
    
    // Bools using for mesh colliders logic
    bool enter1, enter2, exit1, exit2 = false; 
    bool inside = false;


    private void Awake()
    {
        avName = GetHashCode();
        enterAudioVolumeForCrossfades = crossfadesRules.AddAV;
        exitAudioVolumeForCrossfades = crossfadesRules.RemoveAV;
    }

    private void Start()
    {
        //Use public variable to set default reverb level of volume
        ChangeSoundSources(soundSourceData.reverbLevel, soundSourcesReverbLevel);
    }

    private void Update()
    {
        //print(name+" - player inside - "+inside);
    }
    public void StartOnPlayerInside(Collider collider)
    {
        inside = true;
        enterAudioVolume.Invoke(); // Call for any functions that set in inspector on enter audio volume
        enterAudioVolumeForCrossfades(avName); // Call for the function inside AVCrossfades script added this volume name to active list
        print("start player function " + collider.transform.parent.name);
    }

        /*
         This is logic using with mesh colliders to send events on enter and on exit
        */

    public void CollisionOneEnter(Collider soundSourceEntered)
    {
        if (soundSourceEntered.tag != "Player") return; //Notice it reacts only for object with "Player tag"
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
                if (inside) //if a player is inside and exit from the first collider means "outside"
                {
                    OnExitSimpleCollider(soundSourceExited);

                }
                exit2 = false;
            }
        }
    }

    public void CollisionTwoExit(Collider soundSourceExited)
    {
        if (soundSourceExited.tag != "Player") return;
        exit2 = true;
        enter2 = false;
        if (!enter1) { if (exit1) 
            { if (!inside) //if a player is considered outside and exit from the second collider it means "inside"
                {
                    OnEnterSimpleCollider(soundSourceExited);
                }
            exit1 = false; 
            } 
        }
    }

        /*
        This is logic that used for simple colliders and as functions when player inside complex collider
        */

    public void OnEnterSimpleCollider(Collider soundSourceEntered)
    {
        if (soundSourceEntered.tag == "Player")
        {
            inside = true;
            PlayAllSoundSourcesInList();
            enterAudioVolume.Invoke();
            enterAudioVolumeForCrossfades(avName);
        }
    }

    public void OnExitSimpleCollider(Collider soundSourceExited)
    {
        if (soundSourceExited.tag == "Player")
        {
           // print("exit volume " + name + " inside - "+inside);
            inside = false;
            exitAudioVolume.Invoke();
            exitAudioVolumeForCrossfades(avName);
        }
    }

    public void SetPlayerInside(bool insideAV)
    {
        inside = insideAV;
    }

    public void PlayAllSoundSourcesInList()
    {
        {
            foreach (SoundSource sound in soundSources)
            {
                if (!sound.checkedIfPlaying())
                {
                    sound.Play();
                }
                else
                {
                    sound.ResetToDefault();
                }
            }
        }
    }

    // This function is based on enum which is set at the start of the script. You can add new variables
    // and add new logic to this function. 

    public void ChangeSoundSources(soundSourceData dataToChange, float amount)
    {
        switch (dataToChange)
        {
            case soundSourceData.loundness:
                foreach (SoundSource sound in soundSources)
                {
                    sound.ChangeLoudness(amount); // 
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
        if (!inside && zeroLoudnessAfterExit)
        {
            foreach (SoundSource sound in soundSources)
            {
                sound.ResetToZero();
            }
        }
        else
        {
            foreach (SoundSource sound in soundSources)
            {
                sound.ResetToDefault();
            }
        }

    }

    private void OnDestroy()
    {
        enterAudioVolume.RemoveAllListeners();
        exitAudioVolume.RemoveAllListeners();
    }


    public bool isPlayerInside()
    {
        return inside;
    }

    // Return value of this function is used in AVCrossfades script to find the smallest active audio volume
    // to apply its reverb level to sounds inside it and  sounds attached to a player.
    public float ReturnSizeOfAudioVolume()
    {
        Collider col = GetComponentInChildren<Collider>();
        return col.bounds.size.sqrMagnitude;
    }



    /*     * 
  * This functions allows to use curves for fade in and out FMOD events parameters
  * they are not implemented yet
  **/

    IEnumerator FollowCurveFadeIn(float startPoint, float targetPoint, AnimationCurve curve, soundSourceData variableToChange, float fadeSpeed)
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

    IEnumerator FollowCurveFadeOut(float startPoint, float targetPoint, AnimationCurve curve, soundSourceData variableToChange, float fadeSpeed)
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


    /*    public void ChangeSoundsVolumeDynamically(float loudness, float lowpass, float reverb)
    {

        *//* transfer variables to audio volume from other scripts
         * public UnityEvent<float, float, float> toAudioVolume;
         * In the inspector add this function from context menu from the top to send variables here
         *//*

        ChangeSoundSources(soundSourceData.loundness, loudness);
        ChangeSoundSources(soundSourceData.lowpassfreq, lowpass);
        ChangeSoundSources(soundSourceData.reverbLevel, soundSourcesReverbLevel);  // to change reverb dynamically use reverb var 
    }*/

}
