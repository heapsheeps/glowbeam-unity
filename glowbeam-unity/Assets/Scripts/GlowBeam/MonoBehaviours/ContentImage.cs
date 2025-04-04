using UnityEngine;
using System.IO;

namespace GlowBeam
{
    public class ContentImage : ContentBase
    {
        public String filename;

        private Texture2D _texture;

        private new void Awake()
        {
            base.Awake();

            filename = new()
            {
                OnChanged = () => 
                {
                    if (_texture != null)
                        Destroy(_texture);
                    _texture = Utilities.LoadTextureFromDisk(filename.Value);

                    if(_texture == null)
                    {
                        Debug.LogError($"Failed to load texture from: {filename.Value}");
                        return;
                    }

                    SetMainTexture(_texture);
                }
            };

            filename.Value = "/Users/kevin/Downloads/trees.png";
        }

        private void OnDestroy()
        {
            if (_texture != null)
            {
                Destroy(_texture);
                _texture = null;
            }
        }
    }
}
