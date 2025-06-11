// /*
//  *  FILE          :	PromptInfoEditor.cs
//  *  PROJECT       :	JWCProject
//  *  PROGRAMMER    :	Michael Hilts
//  *  FIRST VERSION :	2019-10-20
//  *  DESCRIPTION   : This file contains the PromptInfoEditor class which is responsible for drawing
//  *					the custom editor for the PromptInfo Class.
//  */

// #region Using Statements

// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine;

// #endregion

// /*
//  * CLASS:       PromptInfoEditor
//  * DESCRIPTION: This class is responsible for drawing the custom editor for the PromptInfo Class.
//  */

// [CustomEditor(typeof(PromptInfo))]
// public class PromptInfoEditor : Editor
// {
// 	private bool showSetup = false;		// if true show setup section of editor
// 	private bool showOptions = false;   // if true show options section of editor

// 	#region Editor Callbacks

// 	/*
// 	 * FUNCTION    : OnInspectorGUI()
// 	 * DESCRIPTION : This method is called whenever the inspector GUI must update. It will
// 	 *				 update the entered arrays with new or removed indices.
// 	 * PARAMETERS:
// 	 *		VOID
// 	 * RETURNS     :
// 	 *		VOID
// 	 */

// 	public override void OnInspectorGUI()
// 	{
// 		// get score keeper and draw number of tasks field
// 		PromptInfo pm = target as PromptInfo;

// 		// create style for foldouts
// 		GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
// 		foldoutStyle.fontStyle = FontStyle.Bold;

// 		// if user clicks unity setup foldout, display the setup fields
// 		EditorGUILayout.Separator();
// 		showSetup = EditorGUILayout.Foldout(showSetup, "Unity Setup", true, foldoutStyle);
// 		if (showSetup)
// 		{
// 			DrawDefaultInspector();
// 		}

// 		EditorGUILayout.Separator();
// 		showOptions = EditorGUILayout.Foldout(showOptions, "Prompt Level Options", true, foldoutStyle);

// 		if (showOptions)
// 		{
// 			//pm.numberOfPrompts = EditorGUILayout.IntField("Number of Prompts", pm.numberOfPrompts);

// 			// draw the replacement line
// 			EditorGUILayout.BeginHorizontal();
// 			pm.replacementNum = EditorGUILayout.IntField("Replacement Number", pm.replacementNum);
// 			bool add = false;
// 			bool remove = false;
// 			if (GUILayout.Button("Add"))
// 			{
// 				if (pm.replacementNum > -1 && pm.replacementNum < pm.numberOfPrompts)
// 				{
// 					pm.numberOfPrompts += 1;
// 					add = true;
// 				}
// 			}
// 			if (GUILayout.Button("Remove"))
// 			{
// 				if (pm.replacementNum > -1 && pm.replacementNum < pm.numberOfPrompts)
// 				{
// 					pm.numberOfPrompts -= 1;
// 					remove = true;
// 				}
// 			}
// 			EditorGUILayout.EndHorizontal();

// 			// if the number of tasks has changed, save current values and add to new sized arrays
// 			if (pm.numberOfPrompts != pm.messages.Length)
// 			{
// 				// save current value of arrays
// 				string[] oldMessages = (string[])pm.messages.Clone();
// 				bool[] oldArrowFlags = (bool[])pm.arrowFlags.Clone();
// 				bool[] oldBreadcrumbFlags = (bool[])pm.breadcrumbFlags.Clone();
// 				int[] oldArrowsOn = (int[])pm.arrowsOn.Clone();
// 				int[] oldBreadcrumbsOn = (int[])pm.breadcrumbsOn.Clone();

// 				// create new arrays with new size
// 				pm.messages = new string[pm.numberOfPrompts];
// 				pm.arrowFlags = new bool[pm.numberOfPrompts];
// 				pm.breadcrumbFlags = new bool[pm.numberOfPrompts];
// 				pm.arrowsOn = new int[pm.numberOfPrompts];
// 				pm.breadcrumbsOn = new int[pm.numberOfPrompts];

// 				// determine whether new array is bigger or smaller
// 				int iterations = (pm.numberOfPrompts < oldMessages.Length) ? pm.messages.Length : oldMessages.Length;

// 				// fill in new arrays with as many old values as possible
// 				bool altered = false;
// 				for (int i = 0; i < iterations; i++)
// 				{
// 					if (((add || remove) && !altered) && (i == pm.replacementNum))
// 					{
// 						if (add)
// 						{
// 							pm.messages[i] = string.Empty;
// 							pm.arrowFlags[i] = false;
// 							pm.breadcrumbFlags[i] = false;
// 							pm.arrowsOn[i] = -1;
// 							pm.breadcrumbsOn[i] = -1;
// 							i -= 1;
// 							altered = true;
// 						}
// 						if (remove)
// 						{
// 							pm.messages[i] = oldMessages[i + 1];
// 							pm.arrowFlags[i] = oldArrowFlags[i + 1];
// 							pm.breadcrumbFlags[i] = oldBreadcrumbFlags[i + 1];
// 							pm.arrowsOn[i] = oldArrowsOn[i + 1];
// 							pm.breadcrumbsOn[i] = oldBreadcrumbsOn[i + 1];
// 							altered = true;
// 						}
// 					}
// 					else
// 					{
// 						if (altered)
// 						{
// 							if (add)
// 							{
// 								pm.messages[i + 1] = oldMessages[i];
// 								pm.arrowFlags[i + 1] = oldArrowFlags[i];
// 								pm.breadcrumbFlags[i + 1] = oldBreadcrumbFlags[i];
// 								pm.arrowsOn[i + 1] = oldArrowsOn[i];
// 								pm.breadcrumbsOn[i + 1] = oldBreadcrumbsOn[i];
// 							}
// 							if (remove)
// 							{
// 								pm.messages[i] = oldMessages[i + 1];
// 								pm.arrowFlags[i] = oldArrowFlags[i + 1];
// 								pm.breadcrumbFlags[i] = oldBreadcrumbFlags[i + 1];
// 								pm.arrowsOn[i] = oldArrowsOn[i + 1];
// 								pm.breadcrumbsOn[i] = oldBreadcrumbsOn[i + 1];
// 							}
// 						}
// 						else
// 						{
// 							pm.messages[i] = oldMessages[i];
// 							pm.arrowFlags[i] = oldArrowFlags[i];
// 							pm.breadcrumbFlags[i] = oldBreadcrumbFlags[i];
// 							pm.arrowsOn[i] = oldArrowsOn[i];
// 							pm.breadcrumbsOn[i] = oldBreadcrumbsOn[i];
// 						}
// 					}
// 				}
// 			}

// 			// draw inspector with each tasks fields grouped together
// 			for (int i = 0; i < pm.numberOfPrompts; i++)
// 			{
// 				EditorGUILayout.Separator();
// 				EditorStyles.label.fontStyle = FontStyle.Bold;
// 				EditorGUILayout.LabelField("Prompt " + i.ToString());
// 				EditorStyles.label.fontStyle = FontStyle.Normal;
// 				pm.messages[i] = EditorGUILayout.TextField("Prompt Message", pm.messages[i]);
// 				pm.arrowFlags[i] = EditorGUILayout.Toggle("Progress Arrows", pm.arrowFlags[i]);
// 				if (pm.arrowFlags[i])
// 				{
// 					pm.arrowsOn[i] = EditorGUILayout.IntField("Turn On Arrow", pm.arrowsOn[i]);
// 				}
// 				pm.breadcrumbFlags[i] = EditorGUILayout.Toggle("Progress Breadcrumbs", pm.breadcrumbFlags[i]);
// 				if (pm.breadcrumbFlags[i])
// 				{
// 					pm.breadcrumbsOn[i] = EditorGUILayout.IntField("Turn On Breadcrumb", pm.breadcrumbsOn[i]);
// 				}
// 			}
// 		}

// 		// let editor know changes need to be saved
// 		if (GUI.changed)
// 		{
// 			EditorUtility.SetDirty(pm);
// 			EditorSceneManager.MarkSceneDirty(pm.gameObject.scene);
// 		}
// 	}

// 	#endregion
// }

