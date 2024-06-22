using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class FogEffect : MonoBehaviour
{
    private Transform _transform;
    private Transform _cameraTransform;
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    private Color _fogColor = Color.white;  // 雾的颜色


#if UNITY_EDITOR
    private void OnValidate()
    {
        _transform = transform;
        _cameraTransform = Camera.main.transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
#endif
    private void Start()
    {
        _transform = transform;
        _cameraTransform = Camera.main.transform;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        if (_transform == null || _cameraTransform == null)
            return;
        
        var distance = _transform.position.z - _cameraTransform.position.z;
        var fogDistance = _transform.position.z;
        
        // 线性雾计算公式
        float fogIntensity = Mathf.Clamp(fogDistance / distance, 0, 1);
        
        _spriteRenderer.sharedMaterial.SetFloat("_FogIntensity", fogIntensity);
        _spriteRenderer.sharedMaterial.SetColor("_FogColor", _fogColor);
    }
}
