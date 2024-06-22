using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    // ����ʵ��
    private static Metronome _instance;
    public static Metronome Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("Metronome");
                _instance = obj.AddComponent<Metronome>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }


    public float preBeatEventTime = 0.05f;
    // ÿ��ʱ��
    private float bpm = 120f;
    private float beatInterval;
    private float halfBeatInterval;
    private bool preEventPlayed;

    //
    private float nextFullBeatTime; // ��һ��ȫ��ʱ��
    private float nextHalfBeatTime; // ��һ������ʱ��

    // �¼�
    public static event Action<bool> OnBeatEvent;
    public static event Action PreBeatEvent;

    // ��ǰʱ��
    private float timer = 0f;

    //BGM
    public int StartBGMIndex = 0;
    public bool StartWithBGM = true;
    private bool playingBGM = false;
    private AudioSource audioSource;
    public List<AudioClip> BGMs;
    public List<float> bpms;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        preEventPlayed = false;
    }

    private void Start()
    {
        if(StartWithBGM)    SetBGMOnce(StartBGMIndex);
    }

    private void Update()
    {
        if (playingBGM)
        {
            timer = audioSource.time;
            if (timer >= nextFullBeatTime)
            {
                OnBeatEvent?.Invoke(true);
                //Debug.Log(audioSource.time % beatInterval);
                nextFullBeatTime += beatInterval;
                preEventPlayed = false;
                //nextHalfBeatTime = nextFullBeatTime - halfBeatInterval;
            }
            else if (timer >= nextHalfBeatTime)
            {
                OnBeatEvent?.Invoke(false);
                nextHalfBeatTime += beatInterval;
            }

            if (timer + preBeatEventTime >= nextFullBeatTime && !preEventPlayed)
            {
                PreBeatEvent?.Invoke();
                preEventPlayed = true;
            }
        }
        

        /*
        if (timer >= halfBeatInterval)
        {
            //bool isFullBeat = (Mathf.FloorToInt(timer / halfBeatInterval) % 2 == 0);
            //Debug.Log(isFullBeat);
            OnBeatEvent?.Invoke(isFullBeat);
            if (!isFullBeat)
            {
                preEventPlayed = false;
                Debug.Log(audioSource.time);
                Debug.Log(audioSource.time % halfBeatInterval);
            }
            isFullBeat = !isFullBeat;
            timer -= halfBeatInterval;
        }*/
    }

    private void UpdateIntervals(float newBpm)
    {
        bpm = newBpm;
        beatInterval = 60f / bpm;
        halfBeatInterval = beatInterval / 2f;
    }

    public static float GetBPM()
    {
        return Instance.bpm;
    }

    public float TimeToNextHalfBeat()
    {
        return nextHalfBeatTime - timer;
    }

    public float TimeToNextBeat()
    {
        return nextFullBeatTime - timer;
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private void OnApplicationQuit()
    {
        _instance = null;
    }

    private void OnValidate()
    {

    }

    private void SetBGMOnce(int index)
    {
        audioSource.clip = BGMs[index];
        audioSource.Play();
        timer = 0.0f;
        UpdateIntervals(bpms[index]);
        nextFullBeatTime = halfBeatInterval;
        nextHalfBeatTime = beatInterval;
        //nextFullBeatTime = beatInterval;
        //nextHalfBeatTime = halfBeatInterval;
        playingBGM = true;
    }
}
