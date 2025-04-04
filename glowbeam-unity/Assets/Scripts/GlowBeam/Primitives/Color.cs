using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class Color : Id
    {
        [SerializeField]
        private UnityEngine.Color _value;
        public UnityEngine.Color Value
        {
            get => _value;
            set
            {
                _value = value;
                MarkChanged();
            }
        }

        public Color(UnityEngine.Color value = default) 
            : base() 
        {
            _value = value;
        }
    }
}
