using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using Reaktion;

public class SliderInjector : InjectorBase
{

    public Slider slider;


    public void Update()
    {
        dbLevel = 20.0f * Mathf.Log10(slider.value);

        Debug.Log("dbLevel : " + dbLevel);
    }


}
