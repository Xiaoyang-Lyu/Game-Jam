using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BeatPlayer : MonoBehaviour, IRhythmed
{
    // Start is called before the first frame update
    private SoundEffect halfBeatEffect;
    private SoundEffect fullBeatEffect;
    public AudioClip halfBeatSound;
    public AudioClip fullBeatSound;


    
    
    void Start()
    {
        halfBeatEffect = GetComponent<SoundEffect>();
        if (halfBeatEffect == null){
            halfBeatEffect = gameObject.AddComponent<SoundEffect>();
        }
        halfBeatEffect.soundSource = halfBeatSound;


        fullBeatEffect = GetComponent<SoundEffect>();
        if (fullBeatEffect == null){
            fullBeatEffect = gameObject.AddComponent<SoundEffect>();
        }
        fullBeatEffect.soundSource = fullBeatSound;
        OnEnable();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        Metronome.OnBeatEvent += HandleBeatEvent;
    }

    private void OnDisable()
    {
        Metronome.OnBeatEvent -= HandleBeatEvent;
    }

    private void HandleBeatEvent(bool isFullBeat)
    {
        if (isFullBeat)
        {
            OnBeat();
        }
        else
        {
            OnHalfBeat();
        }
    }

    public void OnHalfBeat()
    {
        halfBeatEffect.PlaySoundEffect();
    }

    public void OnBeat()
    {
        fullBeatEffect.PlaySoundEffect();
    }
    
}
