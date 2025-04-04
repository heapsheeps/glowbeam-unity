using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class Choice<T> : Id
    {
        [SerializeField]
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                _value = value;
                MarkChanged();
            }
        }

        public Choice(T value = default)
            : base()
        {
            _value = value;
        }
    }
}
