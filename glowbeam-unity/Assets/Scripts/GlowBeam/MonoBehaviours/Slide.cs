using UnityEngine;

namespace GlowBeam
{
    public class Slide : MonoBehaviour
    {
        public Bool setToContentDuration;
        public Float duration;
        public Bool loop;
        public Float fadeIn;
        public Float fadeOut;
        public Effect shader;

        private void Awake()
        {
            setToContentDuration = new()
            {
                OnChanged = () => {}
            };

            duration = new()
            {
                OnChanged = () => {}
            };

            loop = new()
            {
                OnChanged = () => {}
            };

            fadeIn = new()
            {
                OnChanged = () => {}
            };
            
            fadeOut = new()
            {
                OnChanged = () => {}
            };

            shader = new Effect();
        }
    }
}
