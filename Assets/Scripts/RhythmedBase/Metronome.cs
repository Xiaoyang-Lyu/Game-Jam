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

    // �ڱ༭���пɱ༭�� BPM ����
    [SerializeField]
    public float bpm = 120f;

    // ÿ��ʱ��
    private float beatInterval;
    private float halfBeatInterval;
    private bool isFullBeat;

    // �¼�
    public static event Action<bool> OnBeatEvent;

    // ��ǰʱ��
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

        // ���� BPM ����ÿ�ĵ�ʱ��
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