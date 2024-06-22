using DG.Tweening;
using System.Collections;
using UnityEngine;


/// <summary>
/// ui元素抖动
/// </summary>
public class UiShake : MonoBehaviour
{
    [SerializeField] private float shakeStrength = 0.5f;
    [SerializeField] private float shakeDuration = 0.5f;
    private bool isShaking;

    private void OnDestroy()
    {
        StopAllCoroutines();
        isShaking = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isShaking = false;
    }

    public void Shake()
    {
        if (isShaking) return;
        isShaking = true;
        StartCoroutine(StartShake());
    }

    IEnumerator StartShake()
    {
        transform.DOShakePosition(shakeDuration, shakeStrength).SetUpdate(true);
        transform.DOShakeRotation(shakeDuration, shakeStrength).SetUpdate(true);
        transform.DOShakeScale(shakeDuration, shakeStrength).SetUpdate(true);
        yield return new WaitForSecondsRealtime(shakeDuration);
        isShaking = false;
    }
}
