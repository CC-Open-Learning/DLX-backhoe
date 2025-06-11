using UnityEngine;

namespace RemoteEducation
{
    /// <summary>A storage class for the hash values of shader properties.</summary>
    public static class ShaderHash
    {
        public static readonly int GlowColor = Shader.PropertyToID("_GlowColor");
        public static readonly int GlowPrePassTex = Shader.PropertyToID("_GlowPrePassTex");
        public static readonly int GlowBlurredTex = Shader.PropertyToID("_GlowBlurredTex");
        public static readonly int BlurredFrame = Shader.PropertyToID("_BlurredFrameTex");
        public static readonly int BlurSize = Shader.PropertyToID("_BlurSize");
        public static readonly int Intensity = Shader.PropertyToID("_Intensity");
        public static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
    }
}
