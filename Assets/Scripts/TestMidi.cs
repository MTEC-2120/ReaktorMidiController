using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class TestMidi : MonoBehaviour
{

    public PianoKey[] pianoKeys = new PianoKey[24];

    public int octave = 4; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // For Testing
        //for(int notenumber=0; notenumber < 128; notenumber++)
        //{
        //    if(MidiDriver.Instance.GetKey(MidiChannel.All, notenumber) >0)
        //    {
        //        Debug.Log("Note number is " + notenumber);
        //    }

        //    if (MidiMaster.GetKeyDown(MidiChannel.All, notenumber))
        //    {
        //        Debug.Log("Note number is " + notenumber);

        //    }


        //}


        for (int i=0; i< (pianoKeys.Length); i++)
        {
            if (MidiDriver.Instance.GetKeyDown(MidiChannel.All, i+60))
            {
                pianoKeys[i].PlayNote();
                pianoKeys[i].gameObject.GetComponent<Animator>().SetBool("down", true);
            }

            if (MidiDriver.Instance.GetKeyUp(MidiChannel.All, i+60))
            {
                pianoKeys[i].gameObject.GetComponent<Animator>().SetBool("down", false);

                AudioSource curr = pianoKeys[i].curr;
                if(curr) StartCoroutine(pianoKeys[i].SoundFade(curr));

            }

        }

    }
}
