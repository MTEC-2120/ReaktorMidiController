using UnityEngine;
using UnityEngine.EventSystems;

public class PianoVolume : MonoBehaviour, /*IPointerEnterHandler, IPointerExitHandler,*/ IPointerDownHandler, IPointerUpHandler
{
    public PianoPitcher pitcher;
    public float vol = 0.5f; //use this value if you need
    bool active;
    float startPos;
    float delta;

    void Update()
    {
        if (active)//this is movement of volume controller
        {
            delta = Input.mousePosition.y - startPos;
            vol = 0.5f-transform.localPosition.z/0.06f;
            transform.localPosition = new Vector3(transform.localPosition.x, 
                transform.localPosition.y,
                Mathf.Clamp(transform.localPosition.z - delta/Screen.height, -0.03f, 0.03f));
            if (vol < 0)
            pitcher.mastervolume = (vol - 0.5f) * 40;
            else
            pitcher.mastervolume = (vol - 0.5f) * 20;
            startPos = Input.mousePosition.y;
        }
        else
            startPos = 0;
        if (Input.GetKeyUp(KeyCode.Mouse0))
            active = false;
    }
    public void OnPointerDown(PointerEventData eventData)//that happens when the volume controller gets pressed
    {
        active = true;
        if (startPos != 0)
            startPos = delta;
        else
            startPos = Input.mousePosition.y;
    }
    public void OnPointerUp(PointerEventData data)//that happens when the volume controller gets unpressed
    {
        delta = Input.mousePosition.y;
    }
}