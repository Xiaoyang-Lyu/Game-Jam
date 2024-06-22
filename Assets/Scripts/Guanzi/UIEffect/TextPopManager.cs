using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;

public class TextPopManager : BehaviourSingleton<TextPopManager>
{
    [SerializeField] private GameObject textPopupPrefab;

    private ObjectPool<GameObject> textPopupPool;

    protected override void Awake()
    {
        base.Awake();
        textPopupPool = new ObjectPool<GameObject>(
            CreatePooledItem,
            OnTakeFromPool,
            OnReturnedToPool,
            OnDestroyPoolObject,
            false, 10, 50);
    }

    private GameObject CreatePooledItem()
    {
        return Instantiate(textPopupPrefab);
    }

    private void OnTakeFromPool(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnReturnedToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyPoolObject(GameObject obj)
    {
        Destroy(obj);
    }

    public void InitPopupText(Vector3 startPosition, Color textColor, int amount)
    {
        InitPopupText(startPosition, textColor, amount.ToString());
    }

    public void InitPopupText(Vector3 startPosition, Color textColor, string text)
    {
        GameObject obj = textPopupPool.Get();
        obj.transform.position = startPosition;
        obj.GetComponent<TextMeshPro>().color = textColor;
        obj.GetComponent<TextMeshPro>().text = text;
        obj.GetComponent<TextPopAnimation>()
            .Popup(() => textPopupPool.Release(obj));
    }
}