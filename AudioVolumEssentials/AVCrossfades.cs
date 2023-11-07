using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//This script I think should be a singleton but maybe it can be used multiple times to make more complicated logic

public class AVCrossfades : MonoBehaviour
{

    public playerCollisionBox player;

    // List of unique crossfade rules, for the moment two same volumes can be used for crossfade only once 
    // I think it should be other approach to this to allow more complicated logic

    [SerializeField]
    List<CrossfadeRule> crossfadesRules = new List<CrossfadeRule>(); 

    AudioVolume[] audioVolumesOnScene; 
    List<int> avNames = new List<int>(); //Check if every volume has unique avName for the same check
    List<float> audioVolumeSizes = new List<float>();
    Dictionary<int, AudioVolume> idToAudioVolumes = new Dictionary<int, AudioVolume>();
    float biggestAudioVolume = 0;

    Dictionary<string, CrossfadeRule> uniqueRuleIds = new Dictionary<string, CrossfadeRule>();

    // List used to check for audio volumes active using their names 
    List<int> avNamesActive = new List<int>(); 

    SoundSource[] soundSourcesOnScene;

    // This is not really good implemented yet, this time it's used on a player only
    List<SoundSource> movableSoundSources = new List<SoundSource>();

    Dictionary<string, int> portalActive = new Dictionary<string, int>();
    int portalKey = 0;
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

        //Check if every audio volume has unique avName
        for (int i = 0; i < audioVolumesOnScene.Length; i++)
        {
            if (avNames.Contains(audioVolumesOnScene[i].avName)) print("The same avName used for different audio volumes, please change avName to unique");
            else avNames.Add(audioVolumesOnScene[i].avName);
        }

        // Check if all rules in crossfade list are unique
        // Also used to link combined avNames to Crossfade rule
        foreach (CrossfadeRule rule in crossfadesRules.ToList())
        {
            if (!uniqueRuleIds.ContainsKey(rule.GetUniqueId()))
            {
                uniqueRuleIds.Add(rule.GetUniqueId(), rule);
                ReplaceRules(rule);
            }
            else { crossfadesRules.Remove(rule); print("There a duplicate in crossfade rules"); }
        }

        // Looping throung all audio volumes to check if player is inside any of them
        // CheckForMesh is very slow function and it can't be used in update method 

        for (int i = 0; i < audioVolumesOnScene.Length; i++)
        {
            CollisionDetection col = audioVolumesOnScene[i].GetComponentInChildren<CollisionDetection>();
            if (col.CheckForMesh(player.transform.position))
            {
                audioVolumesOnScene[i].StartOnPlayerInside(col.GetComponent<Collider>());
            }
        }

        // Making the list of movable sound sources, this should be under loading screen too
        foreach (SoundSource sound in soundSourcesOnScene)
        {
            if (sound.movable)
            {
                movableSoundSources.Add(sound);
            }
        }
    }
    
    // Sorting logic using for sorting and delete non-unique crossfade rules
    void ReplaceRules(CrossfadeRule rule)
    {
        int index = crossfadesRules.FindIndex(s => s == rule);
        crossfadesRules[index] = rule;
    }

    // Check for active audio volumes and if there is a rule can be applied for them
    //
    public void CheckIfRuleApplies(int newAvName)
    {
        if (idToAudioVolumes[newAvName].portal)
        {
            portalKey = newAvName;
            foreach (string key in uniqueRuleIds.Keys)
            {
                if (key.Contains(newAvName.ToString()))
                {
                    portalActive.Add(key, uniqueRuleIds[key].volume01.avName);
                    if (!uniqueRuleIds[key].volume01.isPlayerInside())
                    {
                        uniqueRuleIds[key].volume01.PlayAllSoundSourcesInList();
                        uniqueRuleIds[key].volume01.enterAudioVolume.Invoke();
                        if (!uniqueRuleIds[key].dynamicRule)
                        {
                            ApplyingRule(uniqueRuleIds[key]);
                        }
                        else
                        {
                            uniqueRuleIds[key].soundChanger.OpenGate(uniqueRuleIds[key]);
                        }
                    }
                }
            }
            return;
        }


        if (portalKey != 0)
        {
            print(" how much " + portalActive.Count);
            if (portalActive.Values.Contains(newAvName))
            {
                print("portal has " + newAvName);
                string combinedAVName = GetKeyForUniqueRuleList(newAvName, portalKey);

                if (uniqueRuleIds[combinedAVName].dynamicRule)
                {
                    uniqueRuleIds[combinedAVName].soundChanger.CloseGate();
                    ReturnToDefault(uniqueRuleIds[combinedAVName]);
                    CheckActiveAudioVolumesForRules(newAvName);
                }
                else
                { 
                    ReturnToDefault(uniqueRuleIds[combinedAVName]);
                    CheckActiveAudioVolumesForRules(newAvName);
                }
            }

            foreach (string key in portalActive.Keys)
            {
                print("check " + key);
                if (!uniqueRuleIds[key].volume01.isPlayerInside())
                {
                    uniqueRuleIds[key].volume01.PlayAllSoundSourcesInList();
                    uniqueRuleIds[key].volume01.enterAudioVolume.Invoke();
                    if (!uniqueRuleIds[key].dynamicRule)
                    {
                        ApplyingRule(uniqueRuleIds[key]);
                    }
                    else
                    {
                        uniqueRuleIds[key].soundChanger.OpenGate(uniqueRuleIds[key]);
                    }
                }
            }
            return;
        }

        //Look through names of all active audio volumes add its name 
        CheckActiveAudioVolumesForRules(newAvName);
    }

    private void CheckActiveAudioVolumesForRules(int newAvName)
    {
        foreach (int avName in avNamesActive)
        {
            // Combine names of two volumes to get unique for rule
            string combinedAVName = GetKeyForUniqueRuleList(newAvName, avName);

            if (uniqueRuleIds.ContainsKey(combinedAVName))
            {
                uniqueRuleIds[combinedAVName].active = true;
                if (!uniqueRuleIds[combinedAVName].dynamicRule)
                {
                    ApplyingRule(uniqueRuleIds[combinedAVName]);
                }
                else
                {
                    //Set boolean inside sound changer script's update method to true
                    //to get a float value from it
                    uniqueRuleIds[combinedAVName].soundChanger.OpenGate(uniqueRuleIds[combinedAVName]);
                }
            }
        }
    }

    //Removing active volume from list and reset any rules which are used with it
    void RemoveRulesWithAV(int avName)
    {
        if (idToAudioVolumes[avName].portal)
        {
            foreach(string key in portalActive.Keys)
            {
                if (uniqueRuleIds.TryGetValue(key, out CrossfadeRule ruleExit))
                {
                    ruleExit.active = false;
                    if (ruleExit.dynamicRule)
                    {
                        ruleExit.soundChanger.CloseGate();
                        ReturnToDefault(ruleExit);
                    }
                    ReturnToDefault(ruleExit);
                }
            }
            portalActive.Clear();
            portalKey = 0;
        }

        if (portalKey != 0 && portalActive.Values.Contains(avName))
        {
            foreach (string key in portalActive.Keys)
            {
                if (!uniqueRuleIds[key].volume01.isPlayerInside())
                {
                    uniqueRuleIds[key].volume01.PlayAllSoundSourcesInList();
                    uniqueRuleIds[key].volume01.enterAudioVolume.Invoke();
                    if (!uniqueRuleIds[key].dynamicRule)
                    {
                        ApplyingRule(uniqueRuleIds[key]);
                    }
                    else
                    {
                        uniqueRuleIds[key].soundChanger.OpenGate(uniqueRuleIds[key]);
                    }
                }
                else
                {
                    if (uniqueRuleIds.TryGetValue(key, out CrossfadeRule ruleExit))
                    {
                        ruleExit.active = false;
                        if (ruleExit.dynamicRule)
                        {
                            ruleExit.soundChanger.CloseGate();
                            ReturnToDefault(ruleExit);
                        }
                        ReturnToDefault(ruleExit);
                    }
                }
            }
            return;
        }

        //If there is only 1 active volume every rules are reset
        if (avNamesActive.Count < 2)
        {
            foreach (string key in uniqueRuleIds.Keys)
            {
                if (uniqueRuleIds.TryGetValue(key, out CrossfadeRule ruleExit))
                {
                    ruleExit.active = false;
                    if (ruleExit.dynamicRule)
                    {
                        ruleExit.soundChanger.CloseGate();
                        ReturnToDefault(ruleExit);
                    }
                    ReturnToDefault(ruleExit);
                }
            }
            return;
        }

        foreach (string key in uniqueRuleIds.Keys)
        {
            if (key.Contains(avName.ToString()))
            {
                if (uniqueRuleIds[key].dynamicRule)
                {
                    uniqueRuleIds[key].soundChanger.CloseGate();
                    ReturnToDefault(uniqueRuleIds[key]);
                }
                ReturnToDefault(uniqueRuleIds[key]);
            }
        }
    }

    //Simple logic of making "unique" name of rule

    private string GetKeyForUniqueRuleList(int newAvName, int avName)
    {
        string combinedAVName;
        if (newAvName > avName) //
        {
            combinedAVName = avName.ToString() + newAvName.ToString();
        }
        else
        {
            combinedAVName = newAvName.ToString() + avName.ToString();
        }

        return combinedAVName;
    }

    // This function called from audio volumes when they get player inside 
    public void AddAV(int newAvName)
    {
        if (avNamesActive.Contains(newAvName)) return;

        avNamesActive.Add(newAvName);
/*        print("volume index added " + newAvName);
        print("active volumes - " + avNamesActive.Count);*/
        CheckIfRuleApplies(newAvName);

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
            foreach(SoundSource sound in movableSoundSources)
            {
                sound.ChangeReverb(idToAudioVolumes[smallerAVName].soundSourcesReverbLevel);
            }
        }
    }

    public void RemoveAV(int newAvName)
    {
        avNamesActive.Remove(newAvName);
        RemoveRulesWithAV(newAvName);
        player.ReceiveReverbFromAV(0);
    }

    //Just applying custom loudness and lowpass filter values from rule
    public void ApplyingRule(CrossfadeRule rule)
    {
        //print("applying rule");
        if (!rule.dinamicVol01) rule.volume01.ChangeSoundSources(soundSourceData.loundness, rule.targetLoudnessVol01);
        if (!rule.dinamicVol01) rule.volume01.ChangeSoundSources(soundSourceData.lowpassfreq, rule.targetLowpassVol01);
        if (!rule.dinamicVol02) rule.volume02.ChangeSoundSources(soundSourceData.loundness, rule.targetLoudnessVol02);
        if (!rule.dinamicVol01) rule.volume02.ChangeSoundSources(soundSourceData.lowpassfreq, rule.targetLowpassVol02);
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

    // this can be used only on loding a scene, it's too slow
    public void CheckForMovableInVolume(AudioVolume vol)
    {
        CollisionDetection col = vol.GetComponentInChildren<CollisionDetection>();
        foreach (SoundSource sound in movableSoundSources)
        {
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


/// <summary>
/// This is class defining crossfade rule 
/// </summary>

[Serializable]
public class CrossfadeRule
{
    public bool dynamicRule = false;
    public AudioVolume volume01;
    [Header("This is active only if rule is dynamic")]
    public bool dinamicVol01 = false; 
    public AudioVolume volume02;
    [Header("This is active only if rule is dynamic")]
    public bool dinamicVol02 = false;
    [Range(0, 1)]
    public float targetLoudnessVol01;

    [Range(0, 1)]
    public float targetLowpassVol01;
    [Range(0, 1)]
    public float targetLoudnessVol02;
    [Range(0, 1)]
    public float targetLowpassVol02;
    [Header("This is active only if rule is dynamic")]
    public SoundChanger soundChanger;

    public bool active { get; set; }

    public string GetUniqueId()
    {
        string tempString;
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

