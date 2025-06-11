using System;
using UnityEngine;
using RemoteEducation;

namespace RemoteEducation.Interactions
{
    /// <summary>Handles the initial render pass of the Highlight Camera to retrieve, blur, and isolate the outline.</summary>
    [RequireComponent(typeof(Camera))]
    public class GlowPrePass : MonoBehaviour
    {
        private static RenderTexture PrePass;
        private static RenderTexture Blurred;

        private Material _blurMat;

        private static GlowPrePass singleton;

        private static Camera highlightCamera, parentCamera;
        private static float outlineThickness = 0.5f;

        /// <summary>Unity layer of highlighted objects.</summary>
        /// <remarks>Only relevant when glowing with <see cref="HighlightObject.Mode.OutlineAll"/>.</remarks>
        private static int highlightLayer;
        /// <summary>Last screen width at the time the render textures were calculated.</summary>
        private static int screenWidth;
        /// <summary>Last screen height at the time the render textures were calculated.</summary>
        private static int screenHeight;

        /// <summary>Replacement shader for the Highlight Camera to use.</summary>
        private static Shader glowShader;
        /// <summary>Shader used to blur the output of this camera.</summary>
        private static Shader blurShader;

        /// <summary>If the highlight system's singleton and render textures are fully set up.</summary>
        public static bool Initialized => singleton != null && singleton.initialized;
        private bool initialized = false;

        // This variable is an instance rather than static because we want it to reset to 0 on scene change when a new Highlight Camera is generated.
        private int glows;
        public static int Glows
        {
            get { return singleton.glows; }
            set
            {
                singleton.glows = value;
                CheckGlows();
            }
        }

        void Awake()
        {
            if(singleton != null)
            {
                Debug.LogError("GlowPrePass: Duplicate HighlightCamera object made! Deleting.");
                Destroy(gameObject);
                return;
            }

            singleton = this;
            highlightLayer = 1 << Layers.Highlightable;

            glowShader = Shader.Find("Hidden/GlowReplace");
            blurShader = Shader.Find("Hidden/Blur");
        }

        void OnEnable()
        {
            if (singleton != null)
                RecalculateScreenSize(Screen.width, Screen.height);
        }

        /// <summary>Initializes the GlowCamera RenderTextures, initializes the blur shader, and sets the replacement shaders for a given resolution.</summary>
        private static void RecalculateScreenSize(int width, int height)
        {
            screenWidth = width;
            screenHeight = height;

            if (highlightCamera == null)
            {
                highlightCamera = singleton.GetComponent<Camera>();
            }

            PrePass = new RenderTexture(width, height, 24);
            PrePass.antiAliasing = 8;
            Blurred = new RenderTexture(width >> 1, height >> 1, 0);

            highlightCamera.targetTexture = PrePass;
            highlightCamera.SetReplacementShader(glowShader, "Glowable");
            Shader.SetGlobalTexture(ShaderHash.GlowPrePassTex, PrePass);

            Shader.SetGlobalTexture(ShaderHash.GlowBlurredTex, Blurred);

            singleton._blurMat = new Material(blurShader);
            singleton._blurMat.SetVector(ShaderHash.BlurSize, new Vector2(Blurred.texelSize.x * outlineThickness, Blurred.texelSize.y * outlineThickness));

            singleton.initialized = true;
        }

        private void Update()
        {
            // I hate doing this but we can't modify the Cinemachine code to also set the highlight camera's FOV when it sets the main camera FOV.
            // So instead we set the highlightCamera's FOV every frame to match the main camera.
            highlightCamera.fieldOfView = parentCamera.fieldOfView;
            highlightCamera.nearClipPlane = parentCamera.nearClipPlane;
            highlightCamera.farClipPlane = parentCamera.farClipPlane;

            // If the screen size changed, recalculate our render textures.
            if (screenWidth != Screen.width || screenHeight != Screen.height)
                RecalculateScreenSize(Screen.width, Screen.height);
        }

        // Voodoo magic
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
        }

        /// <summary>Set camera layers and outline width based on the specific object.</summary>
        public static void GlowMode(HighlightObject.Mode outlineMode, float outlineWidth)
        {
            SetOutlineWidth(outlineWidth);

            switch (outlineMode)
            {
                case HighlightObject.Mode.OutlineAll:
                    highlightCamera.cullingMask = highlightLayer;
                    break;

                // ~0 is all 1s in binary.
                case HighlightObject.Mode.OutlineVisible:
                    highlightCamera.cullingMask = ~0;
                    break;
            }
        }

        /// <summary>Sets the camera culling mask to ignore all layers.</summary>
        private static void StopRenderingObjects()
        {
            highlightCamera.cullingMask = 0;
        }

        /// <summary>Sets a reference to the current Highlight System Camera to allow for settings to be changed in response to Glow being called.</summary>
        public static void SetHighlightCameras(Camera highlightCam, Camera parentCam)
        {
            highlightCamera = highlightCam;
            parentCamera = parentCam;
        }

        /// <summary>Sets the blur amount in the shader to increase or decrease the thickness of the outline.</summary>
        public static void SetOutlineWidth(float thickness)
        {
            outlineThickness = thickness;
            singleton._blurMat.SetVector(ShaderHash.BlurSize, new Vector2(Blurred.texelSize.x * thickness, Blurred.texelSize.y * thickness));
        }

        /// <summary>Checks if any objects are currently glowing. Disables all the rendering layers on the Highlight Camera if none are.</summary>
        private static void CheckGlows()
        {
            if (singleton.glows <= 0)
                StopRenderingObjects();
        }
    }
}