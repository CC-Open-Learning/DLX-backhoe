using UnityEngine;

namespace RemoteEducation.Interactions
{
    /// <summary>Sets an object & all its children to be outlined by the Highlight system.</summary>
    public partial class HighlightObject : MonoBehaviour
    {
        public delegate void OnHighlightEvent(HighlightObject highlightObject, bool onOrOff, Color color, float width);
        public delegate void OnHighlightToggledEvent(bool onOrOff);

        public const float OUTLINE_WIDTH_THIN = 0.5f;
        public const float OUTLINE_WIDTH_THICK = 1.5f;

        public static readonly Color HIGHLIGHT_COLOUR = Color.cyan;
        public static readonly Color SELECT_COLOUR = Color.blue;

        public enum Mode
        {
            OutlineAll,
            OutlineVisible
        }

        private enum State
        {
            Idle = 0,
            FadeIn = 1,
            Hold = 2,
            FadeOut = 3
        }
        private State state;

        public Mode OutlineMode = Mode.OutlineVisible;
        public float OutlineWidth;

        // If desired, change these const to be an argument in Glow() so you can set different fade times for different objects.
        private const float FADEINTIME = 0.2f, FADEOUTTIME = 0.1f;

        private Renderer[] renderers;
        private Color glowColor, currentColor, startColor;
        private float t = 0f;

        public bool IsHighlighted { get; private set; }

        public static event OnHighlightEvent OnHighlight;

        /// <summary>
        /// Called when the highlight is turned on or off.</summary>
        public event OnHighlightToggledEvent OnHighLightToggled;

        void Start()
        {
            // We first grab every single material on every single renderer its children have.
            // Afterwards we set a shader tag that our Replacement Shader is looking for in order to tell it that it may affect these objects.
            renderers = GetComponentsInChildren<Renderer>(true);

            for (int i = 0; i < renderers.Length; i++)
            {
                for (int ii = 0; ii < renderers[i].sharedMaterials.Length; ii++)
                {
                    renderers[i].sharedMaterials[ii].SetOverrideTag("Glowable", "True");
                }
            }

            startColor = Color.black;
        }

        private void Update()
        {
            switch(state)
            {
                case State.Idle:
                    return;

                case State.FadeIn:
                    if (!IsHighlighted)
                    {
                        state = State.FadeOut;
                        break;
                    }

                    currentColor = Color.Lerp(startColor, glowColor, t);

                    SetMaterialColors(currentColor);

                    t += Time.deltaTime / FADEINTIME;

                    if (t >= 1)
                    {
                        t = 1;
                        state = State.Hold;
                    }

                    break;

                case State.Hold:
                    if (!IsHighlighted)
                        state = State.FadeOut;

                    break;

                case State.FadeOut:
                    currentColor = Color.Lerp(startColor, glowColor, t);

                    SetMaterialColors(currentColor);

                    t -= Time.deltaTime / FADEOUTTIME;

                    if (t <= 0)
                    {
                        state = State.Idle;

                        t = 0;
                        currentColor = startColor;

                        GlowPrePass.Glows--;

                        SetMaterialColors(currentColor);

                        if (OutlineMode == Mode.OutlineAll)
                            SetLayers(Layers.Default);
                    }
                    break;
            }
        }

        /// <summary>Enables/Disables Highlight around the object.</summary>
        public void Glow(bool onOrOff, Color glowColor = default(Color))
        {
            if (!GlowPrePass.Initialized)
                return;

            if (OnHighlight != null)
                OnHighlight(this, onOrOff, glowColor, OutlineWidth);

            if (!onOrOff || renderers == null || renderers.Length < 1)
            {
                IsHighlighted = false;
                OnHighLightToggled?.Invoke(onOrOff);
                return;
            }

            this.glowColor = glowColor;

            GlowPrePass.GlowMode(OutlineMode, OutlineWidth);

            state = State.FadeIn;

            IsHighlighted = true;

            GlowPrePass.Glows++;

            if (OutlineMode == Mode.OutlineAll)
                SetLayers(Layers.Highlightable);

            OnHighLightToggled?.Invoke(onOrOff);
        }

        /// <summary>Sets the _GlowColor property for all materials in all child renderers.</summary>
        private void SetMaterialColors(Color currentColor)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                for (int ii = 0; ii < renderers[i].materials.Length; ii++)
                {
                    renderers[i].materials[ii].SetColor(ShaderHash.GlowColor, currentColor);
                }
            }
        }

        /// <summary>Sets the Unity Scene layer for all the GameObjects of all child renderers.</summary>
        private void SetLayers(int layer)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].gameObject.layer = layer;
            }
        }

        /// <summary>Adds a <see cref="HighlightObject"/> to the given <paramref name="gameObject"/> if one is not already present.</summary>
        /// <returns>The <see cref="HighlightObject"/> found on or added to <paramref name="gameObject"/>.</returns>
        public static HighlightObject AddOutlineComponent(GameObject gameObject, float outlineWidth)
        {
            HighlightObject outline = gameObject.GetComponent<HighlightObject>();

            if (outline == null)
            {
                outline = gameObject.AddComponent<HighlightObject>();
            }

            outline.OutlineMode = HighlightObject.Mode.OutlineVisible;
            outline.OutlineWidth = outlineWidth;

            return outline;
        }
    }
}
