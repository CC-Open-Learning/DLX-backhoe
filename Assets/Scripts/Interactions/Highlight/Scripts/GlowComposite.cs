using UnityEngine;

namespace RemoteEducation.Interactions
{
    /// <summary>
    /// Determines the Intensity of the additive highlight render texture and creates a composite with the current framebuffer.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class GlowComposite : MonoBehaviour
    {
        // Modify this to change how bright the effect is for the end-user.
        // Left public intentionally so it can be set by other scripts if needed.
        [Range(0, 10)]
        public float Intensity = 3;

        private Material _compositeMat;

        void OnEnable()
        {
            _compositeMat = new Material(Shader.Find("Hidden/GlowComposite"));
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            _compositeMat.SetFloat(ShaderHash.Intensity, Intensity);
            Graphics.Blit(src, dst, _compositeMat, 0);
        }
    }
}