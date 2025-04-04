using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GlowBeam
{
    public class ContentBase : MonoBehaviour
    {
        public String objectName;
        public LiveFloat x;
        public LiveFloat y;
        public LiveFloat width;
        public LiveFloat height;
        public LiveFloat rotation;
        
        public Float start;
        public Float stop;
        public Float fadeIn;
        public Float fadeOut;
        
        public Choice<Perspective> perspective;

        public List<Id> masks;

        public Effect effect;

        private RawImage _rawImage;


        public void Awake()
        {
            GameObject go = gameObject;
            Transform tform = go.transform;
            RectTransform rectTransform = tform as RectTransform;
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(0f, 0f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            objectName = new()
            {
                OnChanged = () => { go.name = objectName.Value; }
            };

            x = new()
            {
                OnChanged = () => 
                { 
                    Vector2 pos = rectTransform.anchoredPosition;
                    pos.x = x.Value;
                    rectTransform.anchoredPosition = pos;
                }
            };

            y = new()
            { 
                OnChanged = () => 
                { 
                    Vector2 pos = rectTransform.anchoredPosition;
                    pos.y = y.Value;
                    rectTransform.anchoredPosition = pos;
                }
            };

            width = new()
            {
                OnChanged = () =>
                {
                    Vector2 size = rectTransform.sizeDelta;
                    size.x = width.Value;
                    rectTransform.sizeDelta = size;
                }
            };

            height = new()
            {
                OnChanged = () =>
                {
                    Vector2 size = rectTransform.sizeDelta;
                    size.y = height.Value;
                    rectTransform.sizeDelta = size;
                }
            };

            rotation = new()
            {
                OnChanged = () =>
                {
                    rectTransform.localEulerAngles = new Vector3(0, 0, rotation.Value);
                }
            };

            start = new()
            {
                OnChanged = () => { /*TODO*/ }
            };

            stop = new()
            {
                OnChanged = () => { /*TODO*/ }
            };

            fadeIn = new()
            {
                OnChanged = () => { /*TODO*/ }
            };

            fadeOut = new()
            {
                OnChanged = () => { /*TODO*/ }
            };

            perspective = new()
            {
                OnChanged = () => { /*TODO*/ }
            };

            //TODO masks

            _rawImage = go.AddComponent<RawImage>();

            effect = new Effect();
            effect.OnMaterialUpdated += () =>
            {
                _rawImage.material = effect.material;
            };
            effect.shaderName.Value = "GlowBeam/Default";
        }

        protected void SetMainTexture(Texture texture)
        {
            _rawImage.texture = texture;

            // Apparently setting the raw image texture does this automatically. But leaving it for clarity.
            // Material material = effect?.material;
            // if (_material == null)
            //     return;
            // material.SetTexture("_MainTex", texture); 
        }
    }
}
