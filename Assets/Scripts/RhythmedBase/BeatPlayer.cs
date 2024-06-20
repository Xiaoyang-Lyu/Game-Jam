using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BeatPlayer : MonoBehaviour, IRhythmed
{
    // Start is called before the first frame update
    //private SoundEffect halfBeatEffect;
    //private SoundEffect fullBeatEffect;
    private AudioSource halfBeatEffect;
    private AudioSource fullBeatEffect;
    public bool playHalfBeatSound;
    public AudioClip halfBeatSound;
    public AudioClip fullBeatSound;
    
    void Start()
    {
        /*
        halfBeatEffect = gameObject.AddComponent<SoundEffect>();
        halfBeatEffect.soundSource = halfBeatSound;

        fullBeatEffect = gameObject.AddComponent<SoundEffect>();
        fullBeatEffect.soundSource = fullBeatSound;
        OnEnable();*/
        halfBeatEffect = GetComponent<AudioSource>();
        if (halfBeatEffect == null)
        {
            halfBeatEffect = gameObject.AddComponent<AudioSource>();
        }

        fullBeatEffect = GetComponent<AudioSource>();
        if (fullBeatEffect == null)
        {
            fullBeatEffect = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        Metronome.PreBeatEvent += OnBeat;
    }

    private void OnDisable()
    {
        Metronome.PreBeatEvent -= OnBeat;
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
        if (playHalfBeatSound)
        {
            halfBeatEffect.PlayOneShot(halfBeatSound);
        }
        //halfBeatEffect.PlaySoundEffect();
    }

    public void OnBeat()
    {
        //fullBeatEffect.PlaySoundEffect();
        fullBeatEffect.PlayOneShot(fullBeatSound);
    }
    
}
