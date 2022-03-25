using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kvant; 

public class ExampleAnimationEvent : MonoBehaviour
{

    public Spray spray;
    public float throttle = 0.5f; 

    public void PrintEvent(string s)
    {
        Debug.Log("PrintEvent: " + s + " called at: " + Time.time);
    }

    public void EmitParticles(float duration)
    {
        StartCoroutine(EmitParticlesForDuration(duration)); 
    }


    private IEnumerator EmitParticlesForDuration(float duration)
    {
        spray.throttle = throttle;
        yield return new WaitForSeconds(duration); 

        yield return null;
        spray.throttle = 0f; 

    }
}
