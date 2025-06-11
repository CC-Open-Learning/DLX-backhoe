using UnityEngine;

namespace RemoteEducation.Interactions
{
    /// <summary>Runs the current rendered frame from the attached <see cref="Camera"/> through a blur filter and masks out the desired areas.</summary>
    [RequireComponent(typeof(Camera))]
    public class FocusComposite : MonoBehaviour
    {
        private RenderTexture Blurred;

        /// <summary>Last screen width at the time the render textures were calculated.</summary>
        private static int screenWidth;
        /// <summary>Last screen height at the time the render textures were calculated.</summary>
        private static int screenHeight;

        private float blurAmount = 0f;

        private Material _blurMat;
        private Material _compositeMat;

        private void Awake()
        {
            Application.targetFrameRate = 144;
            _blurMat = new Material(Shader.Find("Hidden/Blur"));
            _compositeMat = new Material(Shader.Find("Hidden/FocusComposite"));

            RecalculateScreenSize(Screen.width, Screen.height);

            CheckBlurAmount();
        }

        void OnEnable()
        {
            RecalculateScreenSize(Screen.width, Screen.height);
        }

        private void Update()
        {
            // If the screen size changed, recalculate our render textures.
            if (screenWidth != Screen.width || screenHeight != Screen.height)
                RecalculateScreenSize(Screen.width, Screen.height);
        }

        private void RecalculateScreenSize(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;

            Blurred = new RenderTexture(width >> 1, height >> 1, 0);

            _blurMat.SetVector(ShaderHash.BlurSize, new Vector2(Blurred.texelSize.x * blurAmount, Blurred.texelSize.y * blurAmount));

            Shader.SetGlobalTexture(ShaderHash.BlurredFrame, Blurred);
        }

        public void SetBlurAmount(float amount)
        {
            blurAmount = amount;
            _blurMat.SetVector(ShaderHash.BlurSize, new Vector2(Blurred.texelSize.x * blurAmount, Blurred.texelSize.y * blurAmount));

            CheckBlurAmount();
        }
        private void CheckBlurAmount()
        {
            if (blurAmount <= 0)
                enabled = false;
            else if (!enabled)
                enabled = true;
        }

        void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            Graphics.Blit(src, dst);

            Graphics.SetRenderTarget(Blurred);
            GL.Clear(false, true, Color.clear);

            Graphics.Blit(src, Blurred);

            for (int i = 0; i < 4; i++)
            {
                RenderTexture temp = RenderTexture.GetTemporary(Blurred.width, Blurred.height);
                Graphics.Blit(Blurred, temp, _blurMat, 0);
                Graphics.Blit(temp, Blurred, _blurMat, 1);
                RenderTexture.ReleaseTemporary(temp);
            }

            Graphics.Blit(src, dst, _compositeMat, 0);
        }
    }
}