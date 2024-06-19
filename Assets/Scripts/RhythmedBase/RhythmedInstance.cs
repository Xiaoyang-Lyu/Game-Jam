using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmedInstance : MonoBehaviour, IRhythmed
{
    public Vector3 startPosition;
    public Vector3 endPosition;

    private Coroutine moveCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        
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
        //Debug.Log("Half Beat");
    }

    public void OnBeat()
    {
        //Debug.Log("Full Beat");
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            transform.position = startPosition;
        }
        moveCoroutine = StartCoroutine(MoveToPosition());
    }

    private IEnumerator MoveToPosition()
    {
        float bpm = Metronome.GetBPM();
        float beatDuration = 60f / bpm; // 每拍的时间
        float halfBeatDuration = beatDuration / 2f; // 每半拍的时间

        float elapsedTime = 0f;

        while (elapsedTime < halfBeatDuration)
        {
            float t = Mathf.Sin((elapsedTime / halfBeatDuration) * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPosition, endPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;

        elapsedTime = 0f;

        while (elapsedTime < halfBeatDuration)
        {
            float t = Mathf.Sin((elapsedTime / halfBeatDuration) * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(endPosition, startPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = startPosition;
    }
}
