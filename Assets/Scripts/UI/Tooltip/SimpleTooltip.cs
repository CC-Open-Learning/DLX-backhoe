/*
 *  FILE        : SimpleTooltip.cs
 *  PROJECT     : CORE Engine
 *  AUTHOR      : Chowon Jung, Davig Inglis
 *  DESCRIPTION :
 *      Provides a simple tooltip when the mouse cursor is moved over an attached GameObject or UI element
 *      
 *      The SimpleTooltip.cs file is a modification of the SimpleTooltip.cs script
 *      provided in the SimpleTooltip package by 'snorbertas' (https://github.com/snorbertas/simple-tooltip/)
 *      under the MIT License
 */

using UnityEngine;
using UnityEngine.EventSystems;

namespace RemoteEducation.UI.Tooltip
{
    [DisallowMultipleComponent]
    public class SimpleTooltip : FlexibleTooltip, IPointerEnterHandler, IPointerExitHandler
    {
        private EventSystem eventSystem;
        private bool isUIObject = false;

        [HideInInspector]
        public bool ToolTipActive;

        private new void Awake()
        {
            base.Awake();

            ToolTipActive = true;

            eventSystem = FindObjectOfType<EventSystem>();

            if (GetComponent<RectTransform>())
            {
                isUIObject = true;
            }

        }

        private void Update()
        {
            if (!Showing)
            {
                return;
            }

            UpdateTooltip();

            tooltipController.ShowTooltip();
        }

        private void OnMouseEnter()
        {
            if (isUIObject || !ToolTipActive)
            {
                return;
            }

            if (eventSystem)
            {
                if (eventSystem.IsPointerOverGameObject())
                {
                    HideTooltip();
                    return;
                }
            }
            ShowTooltip();
        }

        private void OnMouseExit()
        {
            if (isUIObject)
            {
                return;
            }
            HideTooltip();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!isUIObject || !ToolTipActive)
            {
                return;
            }
            ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!isUIObject)
            {
                return;
            }
            HideTooltip();
        }
    }
}