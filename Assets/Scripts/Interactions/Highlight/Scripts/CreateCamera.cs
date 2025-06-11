using UnityEngine;

namespace RemoteEducation.Interactions
{
    /// <summary>Sets an object & all its children to be outlined by the Highlight system.</summary>
    public partial class HighlightObject : MonoBehaviour
    {
        /// <summary>Instantiates and initializes the second camera needed for the HighlightObject system.</summary>
        public static void CreateHighlightCamera(Camera parentCamera)
        {
            GameObject child = new GameObject("Highlight Camera");

            // Center the child on the parent and set local rotation to 0.
            child.transform.SetParent(parentCamera.transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;

            Camera childCamera = child.AddComponent<Camera>();
            child.AddComponent<DisableLightsForCamera>();

            // Only render objects on the Highlightable layer, otherwise we're just rendering the scene twice.
            childCamera.cullingMask = 1 << Layers.Highlightable;

            // Objects are rendered as pure white on a black background so that the output image of the camera acts as a layer mask.
            childCamera.clearFlags = CameraClearFlags.SolidColor;
            childCamera.backgroundColor = Color.black;

            // We have no lighting, forward rendering makes the most sense.
            childCamera.renderingPath = RenderingPath.Forward;

            // MSAA can be used if desired, however the rendered image will be blurred so aliasing artifacts are not a concern.
            childCamera.allowMSAA = false;
            childCamera.allowHDR = false;

            // Resizing the resolution must call on RecalculateScreenSize, best to do so when the window is finished being resized rather than during.
            childCamera.allowDynamicResolution = false;

            // Copy the rest of the properties from the parent camera.
            childCamera.orthographic = parentCamera.orthographic;
            childCamera.orthographicSize = parentCamera.orthographicSize;
            childCamera.depth = parentCamera.depth;
            childCamera.useOcclusionCulling = parentCamera.useOcclusionCulling;
            childCamera.targetDisplay = parentCamera.targetDisplay;
            childCamera.nearClipPlane = parentCamera.nearClipPlane;
            childCamera.farClipPlane = parentCamera.farClipPlane;
            childCamera.fieldOfView = parentCamera.fieldOfView;
            childCamera.renderingPath = RenderingPath.VertexLit;

            // Add the components that handle the render textures, blurring, compositing, etc.
            parentCamera.gameObject.AddComponent<RemoteEducation.Interactions.GlowComposite>();
            child.AddComponent<RemoteEducation.Interactions.GlowPrePass>();
            GlowPrePass.SetHighlightCameras(childCamera, parentCamera);
        }
    }
}
