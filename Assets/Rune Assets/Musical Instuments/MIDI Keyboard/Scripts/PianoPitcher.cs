using UnityEngine;
using UnityEngine.Audio;

public class PianoPitcher : MonoBehaviour {
    public GameObject piano; //the parent object of all audio sources that PianoKeys spawn
    public AudioClip[] clip; //C of each octave
    public float volume;
    public float mastervolume = 0; //volume of the audio mixer channel
    public AudioMixerGroup group;
    public int octaveOffset;
    public Vector2 octaveOffMinMax; //minimum and maximum octave offset
    public float pitch;
    public PianoBand pitchband;
    float scale = Mathf.Pow(2f, 1.0f / 12f);

    void Start()
    {
        if (piano == null)
            piano = gameObject;
    }
    void Update()
    {
        group.audioMixer.SetFloat("mainvolume", mastervolume); //picks the volume from PianoVolume scripts and sets it to audio mixer
        group.audioMixer.SetFloat("pitch", pitchband.value*-2 + 1); //picks the pitch from PianoBand and sets it to audio mixer
    }
    public void IncreaseOctOff() //increases octave offset
    {
        if (octaveOffset < (int)octaveOffMinMax.y)
        octaveOffset += 1;
    }
    public void DecreaseOctOff() //decreases octave offset
    {
        if (octaveOffset > (int)octaveOffMinMax.x)
            octaveOffset -= 1;
    }
}
