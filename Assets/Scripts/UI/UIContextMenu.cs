using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RemoteEducation.UI
{
    public class UIContextMenu : Editor
    {
        public static readonly string ResourcePathBase = "UI/Templates/";

        static void CreateObject(string resourcePath)
        {
            GameObject clone = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(resourcePath)) as GameObject;

            try
            {
                if (Selection.activeGameObject == null)
                {
                    var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                    Undo.RegisterCreatedObjectUndo(clone, "Created an object");
                    clone.transform.SetParent(canvas.transform, false);
                }

                else
                {
                    Undo.RegisterCreatedObjectUndo(clone, "Created an object");
                    clone.transform.SetParent(Selection.activeGameObject.transform, false);
                }

                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }

            catch
            {
                Undo.RegisterCreatedObjectUndo(clone, "Created an object");
                CreateCanvas();
                var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                clone.transform.SetParent(canvas.transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }

            if (Application.isPlaying == false)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        static void CreateButton(string resourcePath)
        {
            GameObject clone = PrefabUtility.InstantiatePrefab(Resources.Load<GameObject>(resourcePath)) as GameObject;

            try
            {
                if (Selection.activeGameObject == null)
                {
                    var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                    Undo.RegisterCreatedObjectUndo(clone, "Created an object");
                    clone.transform.SetParent(canvas.transform, false);
                }

                else
                {
                    Undo.RegisterCreatedObjectUndo(clone, "Created an object");
                    clone.transform.SetParent(Selection.activeGameObject.transform, false);
                }

                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }

            catch
            {
                Undo.RegisterCreatedObjectUndo(clone, "Created an object");
                CreateCanvas();
                var canvas = (Canvas)GameObject.FindObjectsOfType(typeof(Canvas))[0];
                clone.transform.SetParent(canvas.transform, false);
                clone.name = clone.name.Replace("(Clone)", "").Trim();
            }

            if (Application.isPlaying == false)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        //[MenuItem("GameObject/Modern UI/Canvas", false, -1)]
        static void CreateCanvas()
        {
            GameObject clone = Instantiate(Resources.Load<GameObject>("Other/Canvas"), Vector3.zero, Quaternion.identity) as GameObject;
            Undo.RegisterCreatedObjectUndo(clone, "Created an object");
            clone.name = clone.name.Replace("(Clone)", "").Trim();

            if (Application.isPlaying == false)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }



        [MenuItem("GameObject/CORE UI/Windows/Standard", false, 10)]
        static void Window()
        {
            CreateObject(ResourcePathBase + "Window");

        }

        [MenuItem("GameObject/CORE UI/Windows/Modal", false, 10)]
        static void ModalWindow()
        {
            CreateObject(ResourcePathBase + "Modal");
        }

        [MenuItem("GameObject/CORE UI/Windows/Dialog", false, 10)]
        static void DialogWindow()
        {
            CreateObject(ResourcePathBase + "Dialog");
        }

        [MenuItem("GameObject/CORE UI/Switch", false, 10)]
        static void Switch()
        {
            CreateObject(ResourcePathBase + "Switch");

        }

        [MenuItem("GameObject/CORE UI/Button/Standard", false, 10)]
        static void Button()
        {
            CreateObject(ResourcePathBase + "Buttons/Standard Button");

        }

        [MenuItem("GameObject/CORE UI/Button/Outline", false, 10)]
        static void ButtonOutline()
        {
            CreateObject(ResourcePathBase + "Buttons/Outline Button");
        }

        [MenuItem("GameObject/CORE UI/Button/Radial", false, 10)]
        static void ButtonRadial()
        {
            CreateObject(ResourcePathBase + "Buttons/Radial Button");
        }

        [MenuItem("GameObject/CORE UI/Button/Menu", false, 10)]
        static void ButtonMenu()
        {
            CreateObject(ResourcePathBase + "Buttons/Menu Button");
        }

    }
}
#endif