using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    // 单例实例
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

    // 在编辑器中可编辑的 BPM 属性
    [SerializeField]
    public float bpm = 120f;
    public float preBeatEventTime = 0.05f;
    // 每拍时间
    private float beatInterval;
    private float halfBeatInterval;
    private bool isFullBeat;
    private bool preEventPlayed;

    //
    private float nextFullBeatTime; // 下一个全拍时间
    private float nextHalfBeatTime; // 下一个半拍时间

    // 事件
    public static event Action<bool> OnBeatEvent;
    public static event Action PreBeatEvent;

    // 当前时间
    private float timer = 0f;

    //BGM
    private AudioSource audioSource;
    public List<AudioClip> BGMs;

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

        // 根据 BPM 计算每拍的时间
        UpdateIntervals();
        isFullBeat = false;
        preEventPlayed = false;
    }

    private void Start()
    {
        SetBGMOnce(0);
    }

    private void Update()
    {
        timer = audioSource.time;
        if (timer >= nextFullBeatTime)
        {
            OnBeatEvent?.Invoke(true);
            Debug.Log(audioSource.time % beatInterval);
            nextFullBeatTime += beatInterval;
            preEventPlayed = false;
            //nextHalfBeatTime = nextFullBeatTime - halfBeatInterval;
        }
        else if (timer >= nextHalfBeatTime)
        {
            OnBeatEvent?.Invoke(false);
            nextHalfBeatTime += beatInterval;
        }
        
        if (timer + preBeatEventTime >= nextFullBeatTime && !preEventPlayed )
        {
            PreBeatEvent?.Invoke();
            preEventPlayed = true;
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

    private void UpdateIntervals()
    {
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
        UpdateIntervals();
    }

    private void SetBGMOnce(int index)
    {
        audioSource.clip = BGMs[index];
        audioSource.Play();
        timer = 0.0f;
        nextFullBeatTime = beatInterval;
        nextHalfBeatTime = halfBeatInterval;
    }
}
