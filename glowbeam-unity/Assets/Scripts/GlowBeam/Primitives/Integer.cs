using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class Integer : Id
    {
        [SerializeField]
        private int _value;
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                MarkChanged();
            }
        }

        public Integer(int value = default) 
            : base()
        {
            _value = value;
        }
    }
}
