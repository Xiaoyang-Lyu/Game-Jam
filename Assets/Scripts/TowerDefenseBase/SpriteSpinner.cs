using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSpinner : MonoBehaviour
{
    public float biasX; // X轴的偏移量

    private SpriteRenderer spriteRenderer; // SpriteRenderer 组件

    private void Start()
    {
        // 获取 SpriteRenderer 组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the object.");
        }
        transform.localPosition = new Vector3(biasX, 0, 0);
    }

    // 旋转 sprite 方法
    public void Spin()
    {
        if (spriteRenderer != null)
        {
            // 翻转 sprite 的 X 轴方向
            spriteRenderer.flipX = !spriteRenderer.flipX;

            // 根据 flipX 的值调整 GameObject 的位置
            float positionAdjustment = spriteRenderer.flipX ? -biasX : biasX;
            transform.localPosition = new Vector3(positionAdjustment, 0, 0);
        }
    }
}
