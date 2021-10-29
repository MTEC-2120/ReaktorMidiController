using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reaktion;

public class MidiKeyboardInjector : InjectorBase
{



    private void Start()
    {


        PianoKey.OnPianoKeyDown += PianoKeyDown;
        PianoKey.OnPianoKeyUp += PianoKeyUp;


    }


    public void PianoKeyDown(int note)
    {

            dbLevel = 20.0f * Mathf.Log10(100);

    }


    public void PianoKeyUp(int note)
    {

        dbLevel = 0f;

    }


}
