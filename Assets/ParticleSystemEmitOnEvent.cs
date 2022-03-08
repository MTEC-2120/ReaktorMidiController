using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemEmitOnEvent : MonoBehaviour
{

    public GameObject particlePrefab; 

    ParticleSystem ps; 

    void Start()
    {
        PianoKey.OnPianoKeyDown += Emit;
    }

    void Emit(int var)
    {


        var go = Instantiate(particlePrefab, transform.position, transform.rotation); 
        ps = go.GetComponent<ParticleSystem>();
        ps.startColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        ps.Emit(100*var);

        Destroy(go, 5f); 
       
    }

}
