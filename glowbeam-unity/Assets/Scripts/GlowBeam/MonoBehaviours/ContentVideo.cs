using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

namespace GlowBeam
{
    public class ContentVideo : ContentBase
    {
        public String filename;
        public LiveFloat speed;

        private VideoPlayer _videoPlayer;
        private RenderTexture _renderTexture;

        private new void Awake()
        {
            base.Awake();

            _videoPlayer = gameObject.AddComponent<VideoPlayer>();
            //_videoPlayer.playOnAwake = true;
            _videoPlayer.isLooping = true;
            _videoPlayer.prepareCompleted += (VideoPlayer vp) =>
            {
                if (_renderTexture != null)
                {
                    _renderTexture.Release();
                    Destroy(_renderTexture);
                }
                _renderTexture = new RenderTexture((int)vp.width, (int)vp.height, 0);
                _videoPlayer.targetTexture = _renderTexture;
                SetMainTexture(_renderTexture);
                vp.Play();
            };

            filename = new()
            {
                OnChanged = () => 
                { 
                    _videoPlayer.url = filename.Value;
                    _videoPlayer.Prepare();
                }
            };

            speed = new(1f, 0f, 10f)
            {
                OnChanged = () => 
                { 
                    _videoPlayer.playbackSpeed = speed.Value; 
                }
            };


            filename.Value = "/Users/kevin/Downloads/lf-test-unity/Assets/kevin-tests/video-clips/chunk_reencode_3.mp4";
        }

        private void OnDestroy()
        {
            if (_renderTexture != null)
            {
                _renderTexture.Release();
                Destroy(_renderTexture);
                _renderTexture = null;
            }
        }
    }
}
