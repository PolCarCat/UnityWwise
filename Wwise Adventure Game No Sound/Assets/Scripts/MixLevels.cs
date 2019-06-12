using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public void SetMasterLvl(float masterLvl)
    {
        audioMixer.SetFloat("MasterVol", masterLvl);
    }

    public void SetMusicLvl(float musicLvl)
    {
        audioMixer.SetFloat("MusicVol", musicLvl);
    }
}
