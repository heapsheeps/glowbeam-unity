using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class Float : Id
    {
        [SerializeField]
        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                MarkChanged();
            }
        }

        public readonly float min;
        public readonly float max;

        public Float(float value = default, float min = float.MinValue, float max = float.MaxValue) 
            : base() 
        {
            this.min = min;
            this.max = max;
            _value = Mathf.Clamp(value, min, max); // Ensure the value is within the specified range
        }
    }
}
