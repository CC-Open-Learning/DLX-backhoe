using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VARLab.CloudSave;
using RemoteEducation.Extensions;
using RemoteEducation.Scenarios.Inspectable;

public class GuidGenerator
{
    [MenuItem("Component/SaveLoad/Update GUIDManager Entries")]
    static void Execute()
    {
        var guidManager = GameObject.FindObjectOfType<GuidManager>();

        if (guidManager == null)
        {
            Debug.LogError("No GuidManager found in scene to add entries to.");
            return;
        }

        var allChildren = GameObject.FindObjectsOfType<ElementSpawningData>(true);

        guidManager.PrepareToUpdateEntries();

        foreach (var child in allChildren)
        {
            guidManager.TrackObject(child);
        }

        guidManager.VerifyAllIDsAreUnique();

        EditorUtility.SetDirty(guidManager);
    }
}