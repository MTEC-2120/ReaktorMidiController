using UnityEngine;

public class AudioSyncer : MonoBehaviour
{
    public float Bias
    {
        get => bias;
        protected set => bias = value;
    }

    public float TimeStep
    {
        get => timeStep;
        protected set => timeStep = value;
    }

    public float TimeToBeat
    {
        get => timeToBeat;
        protected set => timeToBeat = value;
    }

    public float RestSmoothTime
    {
        get => restSmoothTime;
        protected set => restSmoothTime = value;
    }

    [SerializeField] private float bias;
    [SerializeField] private float timeStep;
    [SerializeField] private float timeToBeat;
    [SerializeField] private float restSmoothTime;

    private float previousAudioValue;
    private float audioValue;
    private float timer;

    protected bool isBeat;

    private void Update()
    {
        OnUpdate();
    }

    public virtual void OnBeat()
    {

        Debug.Log("OnBeat");
        timer = 0;
        isBeat = true;
    }

    public virtual void OnUpdate()
    {

        //Debug.Log("audioValue " + audioValue);
        previousAudioValue = audioValue;
        audioValue = AudioSpectrum.spectrumValue;

        if (previousAudioValue > bias &&
            audioValue <= bias)
        {
            if (timer > timeStep)
                OnBeat();
        }

        if (previousAudioValue <= bias &&
            audioValue > bias)
        {
            if (timer > timeStep)
                OnBeat();
        }

        timer += Time.deltaTime;
    }
}