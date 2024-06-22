using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;
using DG.Tweening;

/// <summary>
/// 文本跳动动画，用于实现例如伤害数字等的显示
/// </summary>
public class TextPopAnimation : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f; //文本抖动持续时间
    [SerializeField] private float moveDuration = 0.2f; //文本移动持续时间
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshPro textMeshPro;

    private static int _sortingID = 0; //图层顺序

    /// <summary>
    /// 弹出文本
    /// </summary>
    public void Popup(Action releaseFunc)
    {
        _sortingID++;
        textMeshPro.sortingOrder = _sortingID;
        rectTransform.SetParent(null);
        float xMin = -1f; //x移动区间最小
        float xMax = 1f; //x移动区间最大
        float yMin = 2f; //y移动区间最小
        float yMax = 3f; //y移动区间最大
        float xDistance = Random.Range(xMin, xMax);
        float yDistance = Random.Range(xDistance + yMin, xDistance + yMax);
        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 endPos = new Vector2(startPos.x + xDistance, startPos.y + yDistance);
        //动画
        rectTransform.DOAnchorPos(endPos, moveDuration).SetEase(Ease.InSine).OnComplete(() =>
            rectTransform.DOShakePosition(duration, 0.2f).OnComplete(() =>
                textMeshPro.DOFade(0f, duration).From(textMeshPro.color)
                    .OnComplete(() => releaseFunc?.Invoke())));
    }
}
