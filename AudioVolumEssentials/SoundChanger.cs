using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundChanger : MonoBehaviour
{
   // private UnityEvent<float> OnValueChanged = new UnityEvent<float>(); 

    public AVCrossfades crossfades;
    CrossfadeRule rule;
    float changer;
    private float changerCurrent;
    bool gate = false;


    private void Awake()
    {
       // OnValueChanged.AddListener(ChangerFunction);
    }

    public void OpenGate(CrossfadeRule ruleFromCrossfade)
    {
        gate = true;
        rule = ruleFromCrossfade;
    }

    public void CloseGate()
    {
        //print("gate close ");
        gate = false;
        rule = null;
    }

    private void Update()
    {
        if (gate)
        {
            crossfades.ApplyChangesToVolumes(rule, changer);
        }
    }

    public void ChangerFunction(float input) // Method using in moving object to change 
    {
        if (input != changer)
        {
            changer = input;
        }
    }

}
