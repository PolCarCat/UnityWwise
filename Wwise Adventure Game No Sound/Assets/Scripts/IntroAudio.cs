using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAudio : MonoBehaviour
{
    AudioSource source;

    public AudioClip main;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!source.isPlaying)
        {

            source.clip = main;
            source.Play();
            source.loop = true;
        }
    }
}
