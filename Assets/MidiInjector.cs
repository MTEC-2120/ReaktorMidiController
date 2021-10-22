using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reaktion;
using MidiJack;

[AddComponentMenu("Reaktion/Injector/MIDI Injector")]
public class MidiInjector : InjectorBase
{
    // MIDI settings.
    [SerializeField] MidiChannel _midiChannel = MidiChannel.All;
    [SerializeField] int _knobIndex = 2;
    [SerializeField] int _noteNumber = 40;
    public float db_level; 

    //public float DbLevel
    //{
    //    get { return dbLevel; }
    //}

    // Amplitude curve.
    [SerializeField] AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);

    // Default value.
    float _defaultLevel;


    // use midikeys to adjust dbLevel.
    public void Update()
    {
        if (MidiDriver.Instance.GetKeyDown(MidiChannel.All, _noteNumber))
        {
            dbLevel = 20.0f * Mathf.Log10(100);
            db_level = dbLevel;
            Debug.Log("db_level : " + db_level);
        }


        if (MidiDriver.Instance.GetKeyUp(MidiChannel.All, _noteNumber))
        {
            dbLevel = 0f;
            db_level = dbLevel;
            Debug.Log("db_level : " + db_level);

        }

    }

}
