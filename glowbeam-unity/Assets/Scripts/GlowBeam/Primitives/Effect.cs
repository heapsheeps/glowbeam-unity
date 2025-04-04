using System;
using System.Collections.Generic;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class Effect
    {
        public String shaderName;
        public Choice<BlendMode> blendMode;
        public Dictionary<string, object> parameters;

        private Material _material;
        public Material material
        {
            get { return _material; }
        }

        public Action OnMaterialUpdated;

        public Effect()
        {
            shaderName = new();
            shaderName.OnChanged = () => 
            {
                string defaultShader = "GlowBeam/Default";
                SetShader(shaderName.Value ?? defaultShader);
            };

            blendMode = new()
            {
                OnChanged = () => { }
            };
        }

        public void SetShader(string shaderName)
        {
            if (_material != null)
            {
                UnityEngine.Object.Destroy(_material);
                _material = null;
            }

            Shader shader = UnityEngine.Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogWarning("Shader not found: " + shaderName);
                OnMaterialUpdated?.Invoke();
                return;
            }

            _material = new Material(shader);
            
            // Initialize all shader properties
            parameters = new Dictionary<string, object>();
            int propertyCount = shader.GetPropertyCount();
            for (int i = 0; i < propertyCount; i++)
            {
                string paramName = shader.GetPropertyName(i);
                UnityEngine.Rendering.ShaderPropertyType paramType = shader.GetPropertyType(i);
                switch(paramType)
                {
                    case UnityEngine.Rendering.ShaderPropertyType.Float:
                        LiveFloat floatParam = new(_material.GetFloat(paramName));
                        floatParam.Name = paramName;
                        floatParam.OnChanged = () =>
                        {
                            _material.SetFloat(paramName, floatParam.Value);
                        };
                        parameters.Add(paramName, floatParam);
                        break;
                    case UnityEngine.Rendering.ShaderPropertyType.Range:
                        Vector2 range = shader.GetPropertyRangeLimits(i);
                        LiveFloat rangeParam = new(_material.GetFloat(paramName), range.x, range.y);
                        rangeParam.Name = paramName;
                        rangeParam.OnChanged = () =>
                        {
                            _material.SetFloat(paramName, rangeParam.Value);
                        };
                        parameters.Add(paramName, rangeParam);
                        break;
                    case UnityEngine.Rendering.ShaderPropertyType.Int:
                        LiveInteger integerParam = new(_material.GetInteger(paramName));
                        integerParam.Name = paramName;
                        integerParam.OnChanged = () =>
                        {
                            _material.SetInteger(paramName, integerParam.Value);
                        };
                        parameters.Add(paramName, integerParam);
                        break;
                    case UnityEngine.Rendering.ShaderPropertyType.Color:
                        LiveColor colorParam = new(_material.GetColor(paramName));
                        colorParam.Name = paramName;
                        colorParam.OnChanged = () =>
                        {
                            _material.SetColor(paramName, colorParam.Value);
                        };
                        parameters.Add(paramName, colorParam);
                        break;
                    case UnityEngine.Rendering.ShaderPropertyType.Texture:
                        // Unhandled
                        break;
                    case UnityEngine.Rendering.ShaderPropertyType.Vector:
                        // Unhandled
                        break;
                    default:
                        Debug.LogWarning($"Unsupported shader property type: {paramType}");
                        break;
                }
                Debug.Log($"Property {i}: {paramName} of type {paramType}");
            }
            OnMaterialUpdated?.Invoke();
        }
    }
}
