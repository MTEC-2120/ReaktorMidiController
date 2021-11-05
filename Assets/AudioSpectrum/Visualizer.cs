using UnityEngine;
using System.Collections;

public class Visualizer : MonoBehaviour
{
    public GameObject barPrefab;
    public GUIStyle labelStyle;

    public float scaleFactor; 

    SpectrumBar.BarType barType;
    int barCount;

    AudioSpectrum spectrum; 


    private void Start()
    {
        spectrum = GetComponent<AudioSpectrum>();
    }
    void Update()
    {

        LinearSpectrumVisualizer();
        //CircularSpectrumVisualizer();
        //SquareSpectrumVisualizer(); 

    }


    void SquareSpectrumVisualizer()
    {

        int barCountY = 0;

        int barCountX = 0;

        if (barCountX == spectrum.PeakLevels.Length)
        {
            return;
        }

        if (barCountY == spectrum.MeanLevels.Length)
        {
            return;
        }


        //if (barCount == spectrum.Levels.Length)
        //{
        //    return;
        //}

        // Destroy the old bars.
        foreach (var child in transform)
        {
            Destroy((child as Transform).gameObject);
        }

        // Change the number of bars.
        barCountX = spectrum.PeakLevels.Length;
        barCountY = spectrum.MeanLevels.Length;

        var barWidthX = 6.0f / barCountX;
        var barScaleX = new Vector3(barWidthX * 0.9f, 1, -0.75f);


        var barWidthY = 6.0f / barCountY;
        var barScaleY = new Vector3(barWidthY * 0.9f, 1, -0.75f);


        //int ii = (int) Mathf.Sqrt(barCount); 


        // Create new bars.
        for (var j = 0; j < barCountX; j++)
        {
            for (var i = 0; i < barCountY; i++)
            {
                //var x = 6.0f * i / barCount - 3.0f + barWidth / 2;

                //var posX = 5.0f * Mathf.Cos(i * 360 / barCount);
                //var posZ = 5.0f * Mathf.Sin(i * 360 / barCount);


                var posX = 6.0f * i / barCountX - 3.0f + barWidthX / 2;
                var posZ = 6.0f * j / barCountY - 3.0f + barWidthY / 2;


                Vector3 position = new Vector3(posX, 0, posZ);
                var bar = Instantiate(barPrefab, position + 3 * Vector3.forward, transform.rotation) as GameObject;

                bar.GetComponent<SpectrumBar>().index = j;
                bar.GetComponent<SpectrumBar>().barType = barType;


                var scaleX = spectrum.PeakLevels[i];
                var scaleY = spectrum.MeanLevels[i];


                bar.transform.parent = transform;
                bar.transform.localScale = (i * scaleX + j * scaleY) *Vector3.one / scaleFactor; 
            }
        }



    }

    void CircularSpectrumVisualizer()
    {

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

            var posX = 5.0f * Mathf.Cos(i * 360 / barCount);
            var posZ = 5.0f * Mathf.Sin(i * 360 / barCount);

            Vector3 position = new Vector3(posX, 0, posZ); 
            var bar = Instantiate(barPrefab, position + 3 * Vector3.forward, transform.rotation) as GameObject;

            bar.GetComponent<SpectrumBar>().index = i;
            bar.GetComponent<SpectrumBar>().barType = barType;

            bar.transform.parent = transform;
            bar.transform.localScale = barScale;
        }
    }


    void LinearSpectrumVisualizer()
    {

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

            var bar = Instantiate(barPrefab, Vector3.right * x + 3 * Vector3.forward, transform.rotation) as GameObject;

            bar.GetComponent<SpectrumBar>().index = i;
            bar.GetComponent<SpectrumBar>().barType = barType;

            bar.transform.parent = transform;
            bar.transform.localScale = barScale;
        }
    }
}