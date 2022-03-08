using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightManager : MonoBehaviour
{

    public GameObject[] spotLights;
    // Start is called before the first frame update
    void Start()
    {
        PianoKey.OnPianoKeyDown += ChangeLight;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void TurnLightsOff()
    {
        foreach(var lights in spotLights)
        {
            lights.SetActive(false); 
        }
    }
    void ChangeLight(int var)
    {
        TurnLightsOff();
        Debug.Log("var is " + var);

        if (var < spotLights.Length)
        {
            spotLights[var].SetActive(true);
            Color color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            spotLights[var].GetComponentInChildren<Renderer>().material.color = color;
            spotLights[var].GetComponent<Light>().color = color;
        }





    }
}
