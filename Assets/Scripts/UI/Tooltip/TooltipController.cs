/*
 *  FILE        : TooltipController.cs
 *  PROJECT     : CORE Engine
 *  AUTHOR      : Chowon Jung, Davig Inglis
 *  DESCRIPTION :
 *      Controller to provide tooltips when interacting with GameObjects. 
 *      
 *      The TooltipController.cs file is a modification of the STController.cs script
 *      provided in the SimpleTooltip package by 'snorbertas' (https://github.com/snorbertas/simple-tooltip/)
 *      under the MIT License
 */

using UnityEngine;
using UnityEngine.UI;
using RemoteEducation.Helpers;
using TMPro;

namespace RemoteEducation.UI.Tooltip
{
    /// <summary>
    ///     Controller to provide tooltips when interacting with GameObjects
    /// </summary>
    public class TooltipController : MonoBehaviour
    {
        public bool FixedPosition = false;
        public bool Showing { get; private set; } = false;

        private Image panel;
        private TextMeshProUGUI toolTipTextLeft;
        private RectTransform anchor;
        [SerializeField] private RectTransform window;
        private int showInFrames = -1;
        private Vector3 position;

        public static TooltipController Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null)
            {
                Debug.LogWarning("Duplicate TooltipController added to scene, deleting this duplicate instance.");
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // Load up both text layers
            toolTipTextLeft = GetComponentInChildren<TextMeshProUGUI>();

            // Keep a reference for the panel image and transform
            panel = window.GetComponent<Image>();
            anchor = GetComponent<RectTransform>();

            position = Input.mousePosition;

            // Hide at the start
            HideTooltip();
        }

        void Update()
        {
            UpdateShow();
        }

        private void UpdateShow()
        {
            if (Showing)
            {
                anchor.position = GetTooltipPosition();
            }

            if (showInFrames == -1)
            {
                return;
            }

            if (showInFrames == 0)
            {
                Showing = true;
            }

            showInFrames -= 1;
        }

        public void UpdatePosition(Vector3 pos)
        {
            position = pos;
        }

        public void SetRawText(string text)
        {
            toolTipTextLeft.text = text;
        }

        public void SetCustomStyledText(string text, SimpleTooltipStyle style)
        {
            // Update the panel sprite and color
            panel.sprite = style.slicedSprite;
            panel.color = style.color;

            // Update the font asset, size and default color
            toolTipTextLeft.font = style.fontAsset;
            toolTipTextLeft.color = style.defaultColor;

            // Convert all tags to TMPro markup
            var styles = style.fontStyles;
            for (int i = 0; i < styles.Length; i++)
            {
                string addTags = "</b></i></u></s>";
                addTags += "<color=#" + styles[i].color.ToHexString() + ">";
                if (styles[i].bold) addTags += "<b>";
                if (styles[i].italic) addTags += "<i>";
                if (styles[i].underline) addTags += "<u>";
                if (styles[i].strikethrough) addTags += "<s>";
                text = text.Replace(styles[i].tag, addTags);
            }

            toolTipTextLeft.text = text;
        }


        public void ShowTooltip()
        {
            if (!Showing)
            {
                // After 2 frames, showNow will be set to TRUE
                // after that the frame count wont matter
                if (showInFrames == -1)
                    showInFrames = 2;
            }
        }

        public void HideTooltip()
        {
            showInFrames = -1;
            Showing = false;
            anchor.position = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        }


        /// <summary>
        ///     Calculates the appropriate position for the Tooltip anchor point
        ///     based on screen position and the <c>FixedPosition</c> property
        /// </summary>
        /// <returns>Point at which to place the Tooltip anchor</returns>
        public Vector2 GetTooltipPosition()
        {
            Vector2 pos = (FixedPosition) ? position : Input.mousePosition;

            float widthOffset = window.rect.width / 2;

            // Adjust X position offset based on screen width
            if (pos.x + widthOffset > Screen.width)
            {
                pos.x = Screen.width - widthOffset;
            }
            else if (pos.x - widthOffset < 0)
            {
                pos.x = widthOffset;
            }

            // It would be good to validate the Y position is not off-screen as well


            return pos;
        }
    }
}