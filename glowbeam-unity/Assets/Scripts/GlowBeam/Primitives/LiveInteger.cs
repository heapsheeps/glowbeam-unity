using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class LiveInteger : Integer
    {
        private String _oscMapping;

        public LiveInteger(int value = default) 
            : base(value)
        {
            _oscMapping = new String(string.Empty);
        }
    }
}
