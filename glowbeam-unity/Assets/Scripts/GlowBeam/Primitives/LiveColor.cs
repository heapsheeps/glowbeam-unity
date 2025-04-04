using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class LiveColor : Color
    {
        private String _oscMapping;

        public LiveColor(UnityEngine.Color value = default) 
            : base(value)
        {
            _oscMapping = new String(string.Empty);
        }
    }
}
