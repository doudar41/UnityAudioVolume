using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCollisionBox : MonoBehaviour
{
    public List<SoundSource> soundSources = new List<SoundSource>();
    CharacterController player;

    private void Awake()
    {
        player = FindObjectOfType<CharacterController>();
    }


    private void Update()
    {
        if (player.velocity != Vector3.zero)
        {

            foreach(SoundSource sound in soundSources)
            {
                if (!sound.checkedIfPlaying()) sound.Play();
            }
        }
        else
        {
            foreach (SoundSource sound in soundSources)
            {
                sound.Stop();
            }
        }
    }
        
    public void ReceiveReverbFromAV(float amount)
    {
        foreach (SoundSource sound in soundSources)
        {
            //print(sound+" - apply reverb level - " + amount);
            sound.ChangeReverb(amount);
        }
    }


}
