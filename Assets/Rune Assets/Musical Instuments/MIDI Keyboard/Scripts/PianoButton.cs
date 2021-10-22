using UnityEngine;
using UnityEngine.EventSystems;

public class PianoButton : MonoBehaviour, IPointerDownHandler
{
    public AudioSource click;
    public void OnPointerDown(PointerEventData eventData) //the simplest script ever
    {
        click.Play();
    }
}
