using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmedInput : MonoBehaviour
{
    public Text feedbackText; // UI Text ×é¼þ

    public float goodStart = -0.1f;
    public float perfectStart = -0.05f;
    public float perfectEnd = 0.05f;
    public float goodEnd = 0.1f;

    private Coroutine showTextCoroutine;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        float timeToNextBeat = Metronome.Instance.TimeToNextBeat();
        float interval = 60f / Metronome.GetBPM();
        Debug.Log(timeToNextBeat);
        string feedback = GetFeedback(timeToNextBeat, interval);

        ShowFeedback(feedback);
    }

    private string GetFeedback(float timeToNextBeat, float interval)
    {
        float timeFromLastBeat = interval - timeToNextBeat ; // > 0

        if (IsWithinRange(timeFromLastBeat, perfectStart, perfectEnd) || IsWithinRange(-timeToNextBeat, perfectStart, perfectEnd))
        {
            return "Perfect!";
        }
        else if (IsWithinRange(timeFromLastBeat, goodStart, goodEnd) || IsWithinRange(-timeToNextBeat, goodStart, goodEnd))
        {
            return "Good!";
        }
        else
        {
            return "Bad!";
        }
    }

    private bool IsWithinRange(float value, float rangeStart, float rangeEnd)
    {
        return value >= rangeStart && value <= rangeEnd;
    }

    private void ShowFeedback(string feedback)
    {
        feedbackText.text = feedback;
        feedbackText.gameObject.SetActive(true);
        if (showTextCoroutine != null)
        {
            StopCoroutine(showTextCoroutine);
        }
        showTextCoroutine = StartCoroutine(HideFeedbackAfterDelay(0.3f));
    }

    private IEnumerator HideFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        feedbackText.gameObject.SetActive(false);
    }
}
