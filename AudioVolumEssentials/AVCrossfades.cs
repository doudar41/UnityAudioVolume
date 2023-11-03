using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AVCrossfades : MonoBehaviour
{
    public playerCollisionBox player;
    AudioVolume[] audioVolumesOnScene; //Check if every volume has unique avName
    List<int> avNames = new List<int>(); //Check if every volume has unique avName for the same check
    List<float> audioVolumeSizes = new List<float>();
    Dictionary<int, AudioVolume> idToAudioVolumes = new Dictionary<int, AudioVolume>();
    float biggestAudioVolume = 0;
    //
    [SerializeField]
    List<CrossfadeRule> crossfadesRules = new List<CrossfadeRule>(); // List of unique crossfade rules
    Dictionary<string, CrossfadeRule> uniqueIds = new Dictionary<string, CrossfadeRule>();  // Check if all rules in crossfade list are unique
                                                                                            // Also used to link combined avNames to Crossfade rule

    List<int> avNamesActive = new List<int>(); // List used to check for audio volumes active using their names

    SoundSource[] soundSourcesOnScene;
    List<SoundSource> movableSoundSources = new List<SoundSource>();


    private void Awake()
    {
        audioVolumesOnScene = FindObjectsOfType<AudioVolume>();
        soundSourcesOnScene = FindObjectsOfType<SoundSource>();
    }

    private void Start()
    {

        foreach (AudioVolume vol in audioVolumesOnScene)
        {
            audioVolumeSizes.Add(vol.ReturnSizeOfAudioVolume());
            idToAudioVolumes.TryAdd(vol.avName, vol);
        }
        audioVolumeSizes.Sort();
        biggestAudioVolume = audioVolumeSizes.LastOrDefault();



        for (int i = 0; i < audioVolumesOnScene.Length; i++)
        {
            if (avNames.Contains(audioVolumesOnScene[i].avName)) print("The same avName used for different audio volumes, please change avName to unique");
            else avNames.Add(audioVolumesOnScene[i].avName);
        }



        foreach (CrossfadeRule rule in crossfadesRules.ToList())
        {
            if (!uniqueIds.ContainsKey(rule.GetUniqueId()))
            {
                uniqueIds.Add(rule.GetUniqueId(), rule);
                ReplaceRules(rule);
            }
            else { crossfadesRules.Remove(rule); print("There a duplicate in crossfade rules"); }
        }



        for (int i = 0; i < audioVolumesOnScene.Length; i++)
        {
            CollisionDetection col = audioVolumesOnScene[i].GetComponentInChildren<CollisionDetection>();
            if (col.CheckForMesh(player.transform.position))
            {
                audioVolumesOnScene[i].StartOnPlayerInside(col.GetComponent<Collider>());
            }
        }

        foreach (SoundSource sound in soundSourcesOnScene)
        {
            if (sound.Movable)
            {
                movableSoundSources.Add(sound);
            }
        }
    }

    void ReplaceRules(CrossfadeRule rule)
    {
        int index = crossfadesRules.FindIndex(s => s == rule);
        crossfadesRules[index] = rule;
    }


    // MAIN function)) check for active audio volumes if more than 1 proceed, less return every volume to default state
    //
    public void CheckIfRuleApplies(int newAvName)
    {
        //Let's all crossfade rules switch off 
        if (avNamesActive.Count < 2)
        {
            foreach (string key in uniqueIds.Keys)
            {

                if (uniqueIds.TryGetValue(key, out CrossfadeRule ruleExit)) {
                    if (ruleExit.dynamicRule)
                    {
                        ruleExit.soundChanger.CloseGate();
                    }
                    ReturnToDefault(ruleExit); }
            }
            return;
        }

        //Look through names of all active audio volumes add its name and 
        foreach (int avName in avNamesActive)
        {
            string tempString;
            if (newAvName > avName) //
            {
                tempString = avName.ToString() + newAvName.ToString();
            }
            else
            {
                tempString = newAvName.ToString() + avName.ToString();
            }

            if (uniqueIds.ContainsKey(tempString))
            {

                if (!uniqueIds[tempString].dynamicRule)
                {
                    ApplyingRule(uniqueIds[tempString]);
                }
                else
                {
                    uniqueIds[tempString].soundChanger.OpenGate(uniqueIds[tempString]);
                }
            }
            else
            if (uniqueIds.TryGetValue(tempString, out CrossfadeRule ruleExit)) 
            { if (uniqueIds[tempString].dynamicRule)
                {
                    uniqueIds[tempString].soundChanger.CloseGate();
                }
                
                ReturnToDefault(uniqueIds[tempString]);  }
        }
    }

    public void AddAV(int newAvName)
    {
        if (avNamesActive.Contains(newAvName)) return;
        avNamesActive.Add(newAvName);
        CheckIfRuleApplies(newAvName);

        print("active volumes - " + avNamesActive.Count);
        if (avNamesActive.Count > 1)
        {
            float smallerSizeFound = biggestAudioVolume;
            int smallerAVName = 0;
            //Check for audio volume bounce.size smaller has priority for reverb 
            foreach (int name in avNamesActive)
            {
                if (smallerSizeFound > idToAudioVolumes[name].ReturnSizeOfAudioVolume())
                {
                    smallerAVName = idToAudioVolumes[name].avName;
                    smallerSizeFound = idToAudioVolumes[name].ReturnSizeOfAudioVolume();
                }
            }
            print("reverb priority"+idToAudioVolumes[smallerAVName]);
            //player.ReceiveReverbFromAV(idToAudioVolumes[smallerAVName].soundSourcesReverbLevel);
            foreach(SoundSource sound in movableSoundSources)
            {
                sound.ChangeReverb(idToAudioVolumes[smallerAVName].soundSourcesReverbLevel);
            }
        }
    }


    public void RemoveAV(int newAvName)
    {
        avNamesActive.Remove(newAvName);
        CheckIfRuleApplies(newAvName);
        player.ReceiveReverbFromAV(0);
        print("active volumes - " + avNamesActive.Count);
    }

    public void ApplyingRule(CrossfadeRule rule)
    {
        print("applying rule");
        if (!rule.dinamicVol01) rule.volume01.ChangeSoundSources(soundSourceData.loundness, rule.targetLoudnessVol01);
        if (!rule.dinamicVol01) rule.volume01.ChangeSoundSources(soundSourceData.lowpassfreq, rule.targetLowPassVol01);
        if (!rule.dinamicVol02) rule.volume02.ChangeSoundSources(soundSourceData.loundness, rule.targetLoudnessVol02);
        if (!rule.dinamicVol01) rule.volume02.ChangeSoundSources(soundSourceData.lowpassfreq, rule.targetLowPassVol02);
    }

    public void ReturnToDefault(CrossfadeRule rule)
    {
        rule.volume01.ResetSoundsToDefault();
        rule.volume02.ResetSoundsToDefault();
    }


    public void ApplyChangesToVolumes(CrossfadeRule rule, float amount) 
    {

        if (rule.dinamicVol01)
        {
            rule.volume01.ChangeSoundSources(soundSourceData.loundness, amount);
        }
        if (rule.dinamicVol02)
        {
            rule.volume02.ChangeSoundSources(soundSourceData.loundness, amount);
        }
    }

    public void CheckForMovableInVolume(AudioVolume vol)
    {
        CollisionDetection col = vol.GetComponentInChildren<CollisionDetection>();
        foreach (SoundSource sound in movableSoundSources)
        {
            //print(sound.name+ " - " + col.CheckForMesh(sound.gameObject.transform.position)+" difference - " + (Mathf.Abs(col.CheckForMesh(sound.transform.position)) - 12));

              if (col.CheckForMesh(sound.gameObject.transform.position))
            {
                sound.ChangeReverb(vol.soundSourcesReverbLevel);
            }
            else
            {
                sound.ChangeReverb(0);
            }
        }
    }

}

[Serializable]
public class CrossfadeRule
{
    public AudioVolume volume01;
    public bool dinamicVol01 = false;
    public AudioVolume volume02;
    public bool dinamicVol02 = false;
    [Range(0, 1)]
    public float targetLoudnessVol01;
    [Range(0, 1)]
    public float targetLowPassVol01;
    [Range(0, 1)]
    public float targetLoudnessVol02;
    [Range(0, 1)]
    public float targetLowPassVol02;

    public bool dynamicRule = false;
    public SoundChanger soundChanger;

    public string GetUniqueId()
    {
        string tempString;
        //int addedNumbers = 0;
        if (volume01.avName > volume02.avName)
        {
            tempString = volume02.avName.ToString() + volume01.avName.ToString();
        }
        else
        {
            tempString = volume01.avName.ToString() + volume02.avName.ToString();
        }
        return tempString;
    }

    public AudioVolume[] GetVolumesSortedArray()
    {
        if (volume01.avName > volume02.avName)
        {
            return new AudioVolume[2] { volume02, volume01 };
        }
        else
        {
            return new AudioVolume[2] { volume01, volume02 };
        }
    }
}




//Check for SoundSources objects and set parents for them 

/*        for (int i = 0; i < audioVolumesOnScene.Length; i++)
        {
            CollisionDetection col = audioVolumesOnScene[i].GetComponentInChildren<CollisionDetection>();

            foreach (SoundSource sound in soundSourcesOnScene)
            {
                //print(sound.name+ " - " + col.CheckForMesh(sound.gameObject.transform.position)+" difference - " + (Mathf.Abs(col.CheckForMesh(sound.transform.position)) - 12));
                float difference = Mathf.Abs(col.CheckForMesh(sound.transform.position)) - 12;
                if (Mathf.Abs(difference)< 0.6f)
                {
                    if (sound.transform.parent == null)
                    {
                        print(col.transform.parent.name + " - " + sound.name);
                        sound.transform.SetParent(col.transform.parent, true);
                        audioVolumesOnScene[i].AttachEnterAction(sound.Play);
                        audioVolumesOnScene[i].AttachExitAction(sound.Stop);
                        audioVolumesOnScene[i].soundSources = new SoundSource[] { sound } ;
                    }

                }
            }
        }*/