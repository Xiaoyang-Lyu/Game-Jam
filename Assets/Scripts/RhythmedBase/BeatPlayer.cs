using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Unity.Mathematics;
using UnityEngine;

public class BeatPlayer : MonoBehaviour, IRhythmed
{
    // Start is called before the first frame update
    private SoundEffect beatEffect;
    public AudioClip halfBeatSound;
    public AudioClip fullBeatSound;


    
    
    void Start()
    {
        beatEffect = GetComponent<SoundEffect>();
        if (beatEffect == null){
            beatEffect = gameObject.AddComponent<SoundEffect>();
        }
        beatEffect.soundSource = halfBeatSound;


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
        beatEffect.soundSource = halfBeatSound;
        beatEffect.PlaySoundEffect();
    }

    public void OnBeat()
    {
        beatEffect.soundSource = fullBeatSound;
        beatEffect.PlaySoundEffect();
    }
    
}
