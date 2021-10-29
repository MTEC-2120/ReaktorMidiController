using UnityEngine;
using System.Collections;

public class Visualizer : MonoBehaviour
{
    public GameObject barPrefab;
    public GUIStyle labelStyle;

    SpectrumBar.BarType barType;
    int barCount;

    void Update()
    {
        var spectrum = GetComponent<AudioSpectrum>();

        if (barCount == spectrum.PeakLevels.Length)
        {
            return;
        }

        // Destroy the old bars.
        foreach (var child in transform)
        {
            Destroy((child as Transform).gameObject);
        }

        // Change the number of bars.
        barCount = spectrum.PeakLevels.Length;
        var barWidth = 6.0f / barCount;
        var barScale = new Vector3(barWidth * 0.9f, 1, -0.75f);

        // Create new bars.
        for (var i = 0; i < barCount; i++)
        {
            var x = 6.0f * i / barCount - 3.0f + barWidth / 2;

            var bar = Instantiate(barPrefab, Vector3.right * x + 3*Vector3.forward, transform.rotation) as GameObject;

            bar.GetComponent<SpectrumBar>().index = i;
            bar.GetComponent<SpectrumBar>().barType = barType;

            bar.transform.parent = transform;
            bar.transform.localScale = barScale;
        }

    }
}