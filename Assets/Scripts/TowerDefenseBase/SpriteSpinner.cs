using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSpinner : MonoBehaviour
{
    public float biasX; // X���ƫ����

    private SpriteRenderer spriteRenderer; // SpriteRenderer ���

    private void Start()
    {
        // ��ȡ SpriteRenderer ���
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the object.");
        }
        transform.localPosition = new Vector3(biasX, 0, 0);
    }

    // ��ת sprite ����
    public void Spin()
    {
        if (spriteRenderer != null)
        {
            // ��ת sprite �� X �᷽��
            spriteRenderer.flipX = !spriteRenderer.flipX;

            // ���� flipX ��ֵ���� GameObject ��λ��
            float positionAdjustment = spriteRenderer.flipX ? -biasX : biasX;
            transform.localPosition = new Vector3(positionAdjustment, 0, 0);
        }
    }
}
