using UnityEngine;
using UnityEngine.EventSystems;

public class PianoBand : MonoBehaviour, IPointerDownHandler//this script is used to rotate pitch and modulation bands
{
    public float value = 0; //use this value if you need
    bool active;
    float startPos;
    float startValue;
    float delta;

    void Update()
    {
        if (active)
        {
            delta = Input.mousePosition.y - startPos;
            value = Mathf.Clamp(startValue + (-10 * delta / Screen.height), -1, 1);
            transform.localEulerAngles = new Vector3(value * 65, 0, 0);
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
            active = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        active = true;
        startValue = value;
        startPos = Input.mousePosition.y;
    }
}