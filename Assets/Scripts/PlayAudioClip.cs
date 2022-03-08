using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioClip : MonoBehaviour
{




    AudioSource audioSource;
    public AudioClip clip; 


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); 
    }




    // Update is called once per frame
    void Update()
    {
    
        if(Input.GetKeyDown(KeyCode.Space))
        {
            audioSource.clip = clip;
            audioSource.Play(); 


        }

    }
}
