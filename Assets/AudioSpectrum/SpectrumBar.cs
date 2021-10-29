using UnityEngine;
using System.Collections;

public class SpectrumBar : MonoBehaviour
{
    public enum BarType { PeakLevel };

    public int index;
    public BarType barType;

    AudioSpectrum spectrum;

    void Awake()
    {
        spectrum = FindObjectOfType(typeof(AudioSpectrum)) as AudioSpectrum;
    }

    void Update()
    {
        if (index < spectrum.PeakLevels.Length)
        {
            float scale = spectrum.PeakLevels[index];
            var vs = transform.localScale;
            vs.y = scale * 10.0f;
            transform.localScale = vs;
        }
    }
}