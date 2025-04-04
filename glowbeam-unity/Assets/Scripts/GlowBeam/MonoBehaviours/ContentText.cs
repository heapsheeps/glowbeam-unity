using UnityEngine;
using TMPro;

namespace GlowBeam
{
    public class ContentText : ContentBase
    {
        public String text;
        public Choice<HorizontalAlignmentOptions> horizontalAlignment;
        public Choice<VerticalAlignmentOptions> verticalAlignment;

        // TODO: handle font changes

        private TextMeshProUGUI _textObject;

        private new void Awake()
        {
            base.Awake();

            _textObject = gameObject.AddComponent<TextMeshProUGUI>();
            text = new()
            {
                OnChanged = () => { _textObject.text = text.Value; }
            };

            horizontalAlignment = new()
            {
                OnChanged = () => { _textObject.horizontalAlignment = horizontalAlignment.Value; }
            };

            verticalAlignment = new()
            {
                OnChanged = () => { _textObject.verticalAlignment = verticalAlignment.Value; }
            };

        }
    }
}
