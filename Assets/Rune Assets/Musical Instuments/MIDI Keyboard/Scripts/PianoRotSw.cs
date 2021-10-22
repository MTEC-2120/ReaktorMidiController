using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class PianoRotSw : MonoBehaviour, IPointerDownHandler //controls the rotary switches
{
    public float value = 0;
    bool active;
    float startPos;
    float startValue;
    float delta;

    void Update()
    {
        if (active)
        {

            delta = Input.mousePosition.x - startPos;
            value = Mathf.Clamp(startValue + -6 * delta / Screen.width, 0, 1);
            float rot = value * 3 * 90;
            transform.localEulerAngles = new Vector3(0, rot, 0);

            value = rot / 270;
            value = Mathf.Clamp(value, 0, 1);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
            active = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        active = true;
        startValue = value;
        startPos = Input.mousePosition.x;
    }
}