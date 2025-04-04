using System;
using UnityEngine;

namespace GlowBeam
{
    [Serializable]
    public class LiveBool : Bool
    {
        private String _oscMapping;

        public LiveBool(bool value = default) 
            : base(value)
        {
            _oscMapping = new String(string.Empty);
        }
    }
}
