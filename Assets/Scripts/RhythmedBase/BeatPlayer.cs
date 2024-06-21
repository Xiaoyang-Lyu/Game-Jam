using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BeatPlayer : MonoBehaviour, I_Rhythmed
{
    public bool playHalfBeatSound;

    private AudioSource audioSource;

    public List<AudioClip> soundClips;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        Metronome.PreBeatEvent += OnPreBeat;
    }

    private void OnDisable()
    {
        Metronome.PreBeatEvent -= OnPreBeat;
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

    }

    public void OnBeat()
    {
        
    }

    public void OnPreBeat()
    {
        PlayClip(0);
    }

    public void PlayClip(int index)
    {
        audioSource.PlayOneShot(soundClips[index]);
        /*
        audioSource.clip = audioClips[index];
        audioSource.Play();*/
    }

}
