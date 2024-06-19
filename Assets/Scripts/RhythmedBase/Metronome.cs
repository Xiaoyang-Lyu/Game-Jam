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

    // 每拍时间
    private float beatInterval;
    private float halfBeatInterval;
    private bool isFullBeat;

    // 事件
    public static event Action<bool> OnBeatEvent;

    // 当前时间
    private float timer = 0f;

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

        // 根据 BPM 计算每拍的时间
        UpdateIntervals();
        isFullBeat = false;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= halfBeatInterval)
        {
            //bool isFullBeat = (Mathf.FloorToInt(timer / halfBeatInterval) % 2 == 0);
            OnBeatEvent?.Invoke(isFullBeat);
            isFullBeat = !isFullBeat;
            timer -= halfBeatInterval;
        }
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
        return halfBeatInterval - (timer % halfBeatInterval);
    }

    public float TimeToNextBeat()
    {
        return beatInterval - (timer % beatInterval);
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
}
