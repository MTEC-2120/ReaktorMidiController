using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reaktion; 

public class NoiseBallGear : MonoBehaviour
{

    public NoiseBall3 noiseBall; 
    public ReaktorLink reaktor;




    // Update is called once per frame
    void Update()
    {
        var ro = reaktor.Output;
        noiseBall.noiseAmplitude = ro; 

    }
}
