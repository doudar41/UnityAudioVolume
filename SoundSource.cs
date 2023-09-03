using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(StudioEventEmitter))]
public class SoundSource : MonoBehaviour
{
    StudioEventEmitter soundSource;

    private void Awake()
    {
        soundSource = GetComponent<StudioEventEmitter>();
        soundSource.SetParameter("VolumePrivate", 0);
    }

    public void ChangeVolumePlus(AnimationCurve curve,float volume)
    {
        soundSource.EventInstance.getParameterByName("VolumePrivate", out float start);
        StartCoroutine(FollowCurveCoroutingIn(curve, start, volume));
    }

    public void ChangeVolumeMinus(AnimationCurve curve, float volume)
    {
        soundSource.EventInstance.getParameterByName("VolumePrivate", out float start);
        StartCoroutine(FollowCurveCoroutingOut(curve, start, volume));
    }
    public void ChangeLFP(float amount)
    {
        soundSource.SetParameter("LowPassPrivate", amount);
    }

    public void Play(AudioVolume vol, AnimationCurve curve)
    {
        
        if (!soundSource.IsPlaying()) soundSource.Play();
        soundSource.EventInstance.getParameterByName("VolumePrivate", out float start);
        StopCoroutine(FollowCurveCoroutingOut(curve, start, start));
        print("START - " + start);
        StartCoroutine(FollowCurveCoroutingIn(curve, start, vol.thisVolumeCurrentVolume));
    }

    public void Stop(AudioVolume vol, AnimationCurve curve)
    {
        if (!soundSource.IsPlaying()) return;
        soundSource.EventInstance.getParameterByName("VolumePrivate", out float start);
        StopCoroutine(FollowCurveCoroutingIn(curve, start, start));
        print("End - " + start);
        StartCoroutine(FollowCurveCoroutingOut(curve, start, 0));
        
    }

    public bool IsPlaying()
    {
        return soundSource.IsPlaying();
    }

    IEnumerator FollowCurveCoroutingIn(AnimationCurve curve, float startLoudness, float targetLoudness)
    {

        float start = startLoudness;
            while (start < targetLoudness)
            {
                soundSource.SetParameter("VolumePrivate", (curve.Evaluate(start)));
                start += Time.deltaTime;
                yield return new WaitForSeconds(Time.deltaTime * 0.1f);
            }
        soundSource.SetParameter("VolumePrivate", targetLoudness);
        soundSource.EventInstance.getParameterByName("VolumePrivate", out float output);
        print("In - " + output);
        yield return null;
    }

    IEnumerator FollowCurveCoroutingOut(AnimationCurve curve, float startLoudness, float targetLoudness)
    {

        float start = startLoudness;
        while (start > targetLoudness)
        {
            soundSource.SetParameter("VolumePrivate", (curve.Evaluate(start)));
            start -= Time.deltaTime;
            
            yield return new WaitForSeconds(Time.deltaTime * 0.1f);
        }
        soundSource.SetParameter("VolumePrivate", targetLoudness);
        soundSource.EventInstance.getParameterByName("VolumePrivate", out float output);
        print("Out - " + output);
        if (Mathf.Approximately(start, 0)) soundSource.Stop(); 
        yield return null;
    }

    public float GetSourceVolume()
    {
        soundSource.EventInstance.getParameterByName("VolumePrivate", out float start);
        return start;
    }

}
