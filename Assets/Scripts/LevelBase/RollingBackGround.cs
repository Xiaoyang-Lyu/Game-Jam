using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingBackGround : MonoBehaviour
{
    private SpriteRenderer render;
    public float RollingSpeed = 0.01f;
    // Start is called before the first frame update
    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Roll();
    }

    private void Roll()
    {
        if (render != null)
        {
            render.material.mainTextureOffset += new Vector2(RollingSpeed * Time.deltaTime, 0);
        }
    }
}
