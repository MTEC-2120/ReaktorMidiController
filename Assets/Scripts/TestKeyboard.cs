using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MidiJack;

public class TestKeyboard : MonoBehaviour
{

    public PianoKey[] pianoKeys = new PianoKey[12];

    private KeyCode[] keyCodes = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.J, KeyCode.K, KeyCode.L };

    public int octave = 4;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {



       for(int i = 0; i< keyCodes.Length; i++ )
        {

            if (Input.GetKeyDown(keyCodes[i]))
            {
                pianoKeys[i].PlayNote();
                pianoKeys[i].gameObject.GetComponent<Animator>().SetBool("down", true);
            }

            if (Input.GetKeyUp(keyCodes[i]))
            {
                pianoKeys[i].gameObject.GetComponent<Animator>().SetBool("down", false);

                AudioSource curr = pianoKeys[i].curr;
                if (curr) StartCoroutine(pianoKeys[i].SoundFade(curr));

            }

        }





    }
}
