using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;

namespace RemoteEducation.Interactions
{
    public class InputManager : MonoBehaviour
    {
        public class HotkeyData
        {
            public KeyCode Key;
            public Action Function;

            public HotkeyData(KeyCode key, Action function)
            {
                Key = key;
                Function = function;
            }
        }

        private static List<HotkeyData> onKeyDownList;
        private static List<HotkeyData> onKeyHeldList;
        private static List<HotkeyData> onKeyUpList;

        public static bool MouseOverUI
        {
            get
            {
                if (EventSystem.current != null)
                {
                    return EventSystem.current.IsPointerOverGameObject();
                }
                else
                {
                    Debug.LogError("No EventSystem found");

                    return false;
                }
            }
        }

        /// <summary>
        /// Keep track of if the last frame was over a UI object or not.
        /// </summary>
        public bool LastFrameWasOverUI { get; private set; } 


        //Information about the most recent keyboard click
        public static float TimeOnMouseDown { get; private set; }
        public static bool MouseWentDownOnUI;


        static InputManager()
        {
            Refresh();
        }

        /// <summary>
        /// Refresh all the lists that track the functions attached to the key clicks.
        /// This shall be called when the class is first initialized as well as when 
        /// the scenario ends. </summary>
        public static void Refresh()
        {
            onKeyDownList = new List<HotkeyData>();
            onKeyHeldList = new List<HotkeyData>();
            onKeyUpList = new List<HotkeyData>();
        }

        private void Update()
        {
            CheckForKeysDown();
            CheckForKeysHeld();
            CheckForKeysUp();

            CheckOverUITransitions();
            CheckForMouseClick();
        }

        #region Keyboard actions

        private static void CheckForKeysDown()
        {
            for (int i = 0; i < onKeyDownList.Count; i++)
            {
                if (GetKeyDown(onKeyDownList[i].Key))
                    onKeyDownList[i].Function();
            }
        }

        private static void CheckForKeysHeld()
        {
            for (int i = 0; i < onKeyHeldList.Count; i++)
            {
                if (GetKey(onKeyHeldList[i].Key))
                    onKeyHeldList[i].Function();
            }
        }

        private static void CheckForKeysUp()
        {
            for (int i = 0; i < onKeyUpList.Count; i++)
            {
                if (GetKeyUp(onKeyUpList[i].Key))
                    onKeyUpList[i].Function();
            }
        }

        public static void RegisterKeyDown(KeyCode key, Action function)
        {
            RegisterToList(onKeyDownList, key, function);
        }

        public static void RegisterKeyHeld(KeyCode key, Action function)
        {
            RegisterToList(onKeyHeldList, key, function);
        }

        public static void RegisterKeyUp(KeyCode key, Action function)
        {
            RegisterToList(onKeyUpList, key, function);
        }

        private static bool GetKeyDown(KeyCode key)
        {
            // In the future we may want additional conditionals to suspend hotkey usage, they would go here.
            return Hotkeys.GetKeyDown(key);
        }

        private static bool GetKey(KeyCode key)
        {
            // In the future we may want additional conditionals to suspend hotkey usage, they would go here.
            return Hotkeys.GetKey(key);
        }

        private static bool GetKeyUp(KeyCode key)
        {
            // In the future we may want additional conditionals to suspend hotkey usage, they would go here.
            return Hotkeys.GetKeyUp(key);
        }

        /// <summary>
        /// Register a new <see cref="Action"/> to a specified event on a key.
        /// </summary>
        /// <param name="list">The list to register to. This specifies if the event is OnKeyDown/Held/Up</param>
        /// <param name="key">The key the event is registered to.</param>
        /// <param name="function">The action that shall happen on the key event.</param>
        private static void RegisterToList(List<HotkeyData> list, KeyCode key, Action function)
        {
            var existingKey = list.FirstOrDefault(e => e.Key == key);

            if (existingKey != null)
            {
                existingKey.Function = function;
                return;
            }

            list.Add(new HotkeyData(key, function));
        }

        #endregion

        #region Mouse Actions

        /// <summary> Get the object under the mouse </summary>
        /// <returns> The <see cref="GameObject"/> under the mouse. <see langword="null"/> if nothing found</returns>
        public static GameObject GetObjectUnderMouse()
        {
            if (CastRay(out RaycastHit hit))
            {
                return hit.collider.gameObject;
            }

            return null;
        }

        /// <summary> Get the collider under the mouse </summary>
        /// <returns> The <see cref="Collider"/> under the mouse. <see langword="null"/> if nothing found</returns>
        public static Collider GetColliderUnderMouse()
        {
            if(CastRay(out RaycastHit hit))
            {
                return hit.collider;
            }

            return null;
        }

        /// <summary>
        /// Get the world space point under the mouse.
        /// </summary>
        /// <param name="point">The point that was hit.</param>
        /// <param name="layerMask">Layer that will be hit by the raycast. The default is all layers.</param>
        /// <returns>If the ray hit anything.</returns>
        public static bool GetPointUnderMouse(out Vector3 point, int layerMask = ~0)
        {
            if(CastRay(out RaycastHit hit, layerMask))
            {
                point = hit.point;
                return true;
            }

            point = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Cast a ray from the camera to the point on the screen where the mouse is.
        /// </summary>
        /// <param name="hit">The information about what the ray hit.</param>
        /// <param name="layerMask">Layer that will be hit by the raycast. The default is all layers.</param>
        /// <returns>If the ray hit anything.</returns>
        private static bool CastRay(out RaycastHit hit, int layerMask = ~Physics.IgnoreRaycastLayer)
        {
            if(!Camera.main)
            {
                Debug.LogError("Unable to find the Main Camera in the scene");
                hit = new RaycastHit();
                return false;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Constants.MAX_RAYCAST_DISTANCE, layerMask))
            {
                return true;
            }

            return false;
        }

        /// <summary>Gets a specific <see cref="Component"/> on the <see cref="GameObject"/> under the mouse.</summary>
        /// <typeparam name="T">The type of <see cref="Component"/> to get</typeparam>
        /// <param name="checkParent">When <see langword="true"/>, if the <see cref="Component"/> is not found in the <see cref="GameObject"/> under the mouse, its parent will also be checked.</param>
        /// <returns><see cref="Component"/> of type <typeparamref name="T"/>. Returns <see langword="null"/> if there is no <see cref="GameObject"/> under the mouse or if the specified <see cref="Component"/> is not present.</returns>
        public static T GetComponentUnderMouse<T>(bool checkParent = true) where T : MonoBehaviour
        {
            T[] undermouse = GetComponentsUnderMouse<T>(checkParent);

            return undermouse == null || undermouse.Length == 0 ? null : undermouse[0];
        }


        /// <summary>Gets all the <see cref="Component"/>s on the <see cref="GameObject"/> under the mouse.</summary>
        /// <typeparam name="T">The type of <see cref="Component"/> to get</typeparam>
        /// <param name="checkParent">When <see langword="true"/>, if no <see cref="Component"/>s are found in the <see cref="GameObject"/> under the mouse, its parent will also be checked.</param>
        /// <returns><see cref="Component"/> of type <typeparamref name="T"/>. Returns <see langword="null"/> if there is no <see cref="GameObject"/> under the mouse or if the specified <see cref="Component"/> is not present.</returns>
        public static T[] GetComponentsUnderMouse<T>(bool checkParent = true) where T : MonoBehaviour
        {
            GameObject obj = GetObjectUnderMouse();

            if (obj != null)
            {
                T[] underMouse = obj.GetComponents<T>();

                if(underMouse == null && checkParent)
                {
                    underMouse = obj.GetComponentsInParent<T>();
                }

                return underMouse;
            }

            return null;
        }

        /// <summary>Handles the transition that occurs when a user's mouse moves to/from an <see cref="Interactable"/> and a UI element in the same frame.</summary>
        public void CheckOverUITransitions()
        {
            bool mouseCurrentlyOverUI = MouseOverUI;

            if(LastFrameWasOverUI != mouseCurrentlyOverUI)
            {
                Interactable.UpdateHoveringOnUITransition(!LastFrameWasOverUI);

                LastFrameWasOverUI = mouseCurrentlyOverUI;
            }
        }

        /// <summary> Catch all mouse up/down events. Keep track of mouse down event data. </summary>
        /// <remarks><see cref="Interactable.GeneralMouseUp"/> is called here.</remarks>
        public void CheckForMouseClick()
        {
            if (Input.GetMouseButtonDown(Constants.LEFT_MOUSE_BUTTON))
            {
                TimeOnMouseDown = Time.time;
                MouseWentDownOnUI = MouseOverUI;
            }
            else if (Input.GetMouseButtonUp(Constants.LEFT_MOUSE_BUTTON))
            {
                Interactable.GeneralMouseUp();
            }
        }

        #endregion
    }
}
