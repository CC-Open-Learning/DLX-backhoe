/*
 * Controls the presentation of Windows and the Buttons which call them into view. 
 * 
 * Responsibilities of each individual submenu are left to them; this class manages 
 * the aggregation of these Windows and their Buttons
 *		
 */

#region Resources

using System.Collections.Generic;
using UnityEngine;
using Lean.Gui;

#endregion

namespace RemoteEducation.UI
{

    /// <summary>
    ///     Manages multiple <see cref="LeanWindow"/> objects to present UI windows to a user
    /// </summary>
    public class WindowManager : MonoBehaviour
    {

        /// <summary>
        ///     Stores the collection of LeanWindows managed by the WindowManager
        /// </summary>
        private List<LeanWindow> windows;

        [Header("UI Settings")]
        public RectTransform ButtonGroup;
        public RectTransform WindowGroup;

        [Header("Specialized Windows")]
        public Dialog DialogWindow;
        public Dialog LoadDialogWindow;

        public LeanWindowCloser WindowCloser { get; private set; }

        /// <summary>
        ///     Initializes the "windows" Collection
        /// </summary>
        private void Awake()
        {
            windows = new List<LeanWindow>();
            WindowCloser = GetComponent<LeanWindowCloser>();

            SetButtonGroupVisible(false);
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="windowObject"></param>
        /// <param name="buttonObject"></param>
        public void Add(GameObject windowObject, GameObject buttonObject = null)
        {
            if (windowObject)
            {
                windowObject.transform.SetParent(WindowGroup, false);
                Add(windowObject.GetComponentInChildren<LeanWindow>(), 
                    buttonObject != null ? buttonObject.GetComponentInChildren<LeanButton>() : null,
                    false);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        public void Add(WindowLoader loader)
        {
            if (loader)
            {
                Add(loader.Window, loader.Button);
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="window"></param>
        /// <param name="button"></param>
        /// <param name="setWindowParent"></param>
        /// <param name="setButtonParent"></param>
        public void Add(LeanWindow window, LeanButton button = null, bool setWindowParent = true, bool setButtonParent = true)
        {
            if (!window)
            {
                return;
            }

            // Add window to managed collection
            windows.Add(window);
            if (setWindowParent)
            {
                window.transform.SetParent(WindowGroup, false);
            }

            if (button)
            {
                if (setButtonParent)
                {
                    button.transform.SetParent(ButtonGroup, false);
                }

                button.OnClick.AddListener(delegate { window.Toggle(); });
            }

            window.On = false;
        }


        /// <summary>
        ///     Set the visibility of the WindowManager button group
        /// </summary>
        /// <param name="visible"></param>
        public void SetButtonGroupVisible(bool visible)
        {
            ButtonGroup.gameObject.SetActive(visible);
        }

    }
}
