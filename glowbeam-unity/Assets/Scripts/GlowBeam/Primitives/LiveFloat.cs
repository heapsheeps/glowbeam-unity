using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class LiveFloat : Float
    {
        private String _oscMapping;

        public LiveFloat(float value = default, float min = float.MinValue, float max = float.MaxValue) 
            : base(value, min, max)
        {
            _oscMapping = new String(string.Empty);
        }
    }
}
