using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class Bool : Id
    {
        [SerializeField]
        private bool _value;
        public bool Value
        {
            get => _value;
            set
            {
                _value = value;
                MarkChanged();
            }
        }

        public Bool(bool value = default)
            : base()
        {
            _value = value;
        }
    }
}
