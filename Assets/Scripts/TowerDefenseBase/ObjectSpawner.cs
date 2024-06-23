using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public List<GameObject> objectPrefab;
    public Stage stage;
    private GameObject currentObject;

    // 调用这个方法来生成预制体
    public void CreateObject(int index)
    {
        if (currentObject != null) return;
        currentObject = Instantiate(objectPrefab[index]);
        UpdateObjectPosition();
    }

    void Update()
    {
        if (currentObject == null) return;

        // 更新位置
        UpdateObjectPosition();

        // 确认放置
        if (Input.GetMouseButtonDown(0)) // 鼠标左键
        {
            Vector3 newPosition = currentObject.transform.position;
            if (stage.GetNearestCell(ref newPosition))
            {
                currentObject.transform.position = newPosition;
                currentObject = null; // 放置完成，清除引用
            }
            else
            {
                Destroy(currentObject); // 销毁对象
                currentObject = null; // 清除引用
            }

        }

        // 取消操作
        if (Input.GetMouseButtonDown(1)) // 鼠标右键
        {
            Destroy(currentObject); // 销毁对象
            currentObject = null; // 清除引用
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SpriteSpinner spinner = currentObject.GetComponentInChildren<SpriteSpinner>();
            if(spinner != null)
            {
                spinner.Spin();
            }
        }
    }

    void UpdateObjectPosition()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currentObject.transform.position = mousePosition;
    }
}
