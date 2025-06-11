using RemoteEducation.Helpers.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Interactions
{
    /// <summary>Wrapper for the Unity Cursor class.</summary>
    public static class CursorIcons
    {
        public enum Cursors
        {
            Default = 0,
            HandOpen = 1,
            HandClosed = 2,
            HandPointer = 3
        }

        public static Cursors current { get; private set; }

        public static bool DisableCustomCursors = false;

        private static Dictionary<Cursors, Texture2D> cursors;

        static CursorIcons()
        {
            cursors = new Dictionary<Cursors, Texture2D>();

            // Get all the names/values of the Cursors enum and see if we can find a Texture of the same name in ../Resources/UI/Cursors.
            var enumNames = Enum.GetNames(typeof(Cursors));
            var enumValues = (Cursors[])Enum.GetValues(typeof(Cursors));

            Texture2D tex;

            for (int i = 0; i < enumNames.Length; i++)
            {
                // Ignore default case.
                if (enumValues[i] == Cursors.Default)
                    continue;

                tex = Resources.Load("UI/Cursors/" + enumNames[i]) as Texture2D;

                if (tex == null)
                {
                    Debug.LogError("<color=#0099ff>CursorIcons</color> : Missing texture for cursor " + enumNames[i] + " in Resources/UI/Cursors!");
                    continue;
                }

                cursors.Add(enumValues[i], tex);
            }

            current = Cursors.Default;
        }

        public static void Init()
        {
            FadeSceneLoader.OnLoadScene += FadeSceneLoader_OnLoadScene;
        }

        private static void FadeSceneLoader_OnLoadScene(string sceneName)
        {
            SetCursor(Cursors.Default);

            // When a scene is loaded, we set this to false.
            // The Extension Module for a scene may set this to true if needed.
            DisableCustomCursors = sceneName.Equals(Constants.START_SCENE_NAME);

            Debug.Log("Enabling custom cursors");
        }

        /// <summary>Sets the cursor for the end-user to any of the custom cursors.</summary>
        public static void SetCursor(Cursors cursor)
        {
            // Set the cursor to the OS default.
            if(DisableCustomCursors || cursor == Cursors.Default)
            {
                current = Cursors.Default;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                return;
            }

            Texture2D tex;

            if(!cursors.TryGetValue(cursor, out tex))
            {
                Debug.LogError("<color=#0099ff>CursorIcons</color> : SetCursor called for missing cursor " + cursor.ToString() + "!");
                return;
            }

            current = cursor;
            Cursor.SetCursor(tex, new Vector2(14, 2), CursorMode.Auto);
        }
    }
}