using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reaktion; 

public class AudioListenerInjector : InjectorBase
{

    #region Private variables
    float[] rawSpectrum;
    int numberOfSamples = 64;

    #endregion
    // Start is called before the first frame update
    void Start()
    {
        if (rawSpectrum == null || rawSpectrum.Length != numberOfSamples)
        {
            rawSpectrum = new float[numberOfSamples];
        }
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.GetSpectrumData(rawSpectrum, 0, FFTWindow.BlackmanHarris);


        dbLevel = 20.0f * Mathf.Log10(rawSpectrum[0]);

        Debug.Log("dbLevel " + dbLevel);
    }
}
