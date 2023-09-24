using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AudioVolume : MonoBehaviour
{

    public UnityEvent enterAudioVolume;
    public UnityEvent exitAudioVolume;

    bool enter1, enter2, exit1, exit2 = false;

    public void CollisionOneEnter()
    {
        enter1 = true;
        exit1 = false;
    }
    public void CollisionTwoEnter()
    {
        enter2 = true;
        exit2 = false;
    }
    public void CollisionOneExit()
    {
        exit1 = true;
        enter1 = false;
        if (!enter2)
        {
            if (exit2) {  exitAudioVolume.Invoke(); exit2 = false; }
        }
    }

    public void CollisionTwoExit()
    {
        exit2 = true;
        enter2 = false;
        if (!enter1) { if (exit1) { enterAudioVolume.Invoke(); exit1 = false; } }
    }


    public void  EnterVolume()
    {
        Debug.Log("enter volume");
    }

    public void ExitVolume()
    {
        Debug.Log("exit  volume");
    }
}
