using UnityEngine;

namespace GlowBeam
{
    public class Mask : MonoBehaviour
    {
        public Float feather;
        public Float offset;
        public String filename;

        private Texture2D _maskTexture;

        private void Awake()
        {
            feather = new()
            {
                OnChanged = () => {}
            };

            offset = new()
            {
                OnChanged = () => {}
            };

            filename = new()
            {
                OnChanged = () => 
                {
                    _maskTexture = Utilities.LoadTextureFromDisk(filename.Value);
                    if(_maskTexture == null)
                    {
                        Debug.LogError($"Failed to load texture from: {filename.Value}");
                        return;
                    }

                    // TODO set the texture to the common.effect somehow
                }
            };
        }

        // private void LoadMask()
        // {
        //     // Stub loading code:
        //     Texture2D loaded = new Texture2D(2, 2, TextureFormat.RFloat, false);
        //     mask = new RenderTexture(loaded.width, loaded.height, 0, RenderTextureFormat.RFloat);
        //     Graphics.Blit(loaded, mask);
        // }
    }
}
