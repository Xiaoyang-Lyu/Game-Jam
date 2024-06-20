using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip soundSource;
    private AudioSource audioPlayer;
    // Start is called before the first frame update
    void Start()
    {
        
    // add  audio component
        audioPlayer = GetComponent<AudioSource>();
        if (audioPlayer == null)
        {
            audioPlayer = gameObject.AddComponent<AudioSource>();
        } 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlaySoundEffect()
    {
        // 播放音效
        audioPlayer.PlayOneShot(soundSource);
    }
}
