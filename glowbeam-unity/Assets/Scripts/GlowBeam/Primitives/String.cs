using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class String : Id
    {
        [SerializeField]
        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                MarkChanged();
            }
        }

        public String(string value = default)
            : base()
        {
            _value = value;
        }
    }
}
