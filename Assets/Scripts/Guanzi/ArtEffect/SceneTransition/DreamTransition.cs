using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GrayCity
{
    public class DreamTransition : MonoBehaviour
    {
        private RenderTexture renderTextureFrom;    //第一个场景的相机的渲染纹理
        private RenderTexture renderTextureTo;      //第二个场景的相机的渲染纹理

        [SerializeField] private Camera cameraFrom;     //第一个场景中用于捕捉渲染纹理的相机，需要提前在场景中设置好，把Camera组件的enabled设置为false，同时去除transition层
        private Camera cameraTo;    //第二个场景中用于捕捉渲染纹理的相机，需要提前在场景中设置好，把Camera组件的enabled设置为false，同时去除transition层，这个相机需要挂上ITransitionCamera用于自动获取

        [SerializeField] private Shader transitionShader;   //渲染纹理转换到目标纹理的shader
        
        [SerializeField] private string fromSceneName;
        [SerializeField] private string toSceneName;

        private float _timer = 0f;
        [SerializeField] private float transitionTime = 2f;
        
        private bool _isTransitioning = false;   //确保场景二加载完毕后再开始过渡

        [SerializeField] private SpriteRenderer shower;     //用于显示渲染纹理的SpriteRenderer，技术原因，只能用SpriteRenderer，不能用Image

        public IEnumerator PerformTransition()
        {
            renderTextureFrom = new RenderTexture(Screen.width, Screen.height, 0);
            renderTextureTo = new RenderTexture(Screen.width, Screen.height, 0);

            cameraFrom.targetTexture = renderTextureFrom;
            cameraFrom.GetComponent<Camera>().enabled = true;

            yield return SceneManager.LoadSceneAsync(toSceneName, LoadSceneMode.Additive);

            cameraTo = FindObjectsOfType<TransitionEffectCamera>().First().GetComponent<Camera>();
            cameraTo.targetTexture = renderTextureTo;
            cameraTo.GetComponent<Camera>().enabled = true;
            
            yield return new WaitForSeconds(5f);
            _isTransitioning = true;
        }

        private void Update()
        {
            if(_isTransitioning)
                _timer += Time.deltaTime;
                ShowTextureToTarget();
        }

        private void ShowTextureToTarget()
        {
            var material = new Material(transitionShader);
            material.SetTexture("_TextureA", renderTextureFrom);
            material.SetTexture("_TextureB", renderTextureTo);
            material.SetFloat("_Progress", _timer/transitionTime);
            
            shower.material = material;
            shower.enabled = true;

            if (_timer >= transitionTime)
            {
                cameraFrom.targetTexture = null;
                cameraTo.targetTexture = null;
                cameraFrom.enabled = false;
                cameraTo.enabled = false;
                Destroy(renderTextureFrom);
                Destroy(renderTextureTo);
                
                shower.enabled = false;
                
                SceneManager.UnloadSceneAsync(fromSceneName);
                Destroy(gameObject);
            }
        }
    }
}
