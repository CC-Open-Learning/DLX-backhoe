using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using RemoteEducation.Extensions;
using RemoteEducation.Scenarios.Inspectable;
using UnityEditor.SceneManagement;

public class DisableLightProbes
{
    [MenuItem("Component/Rendering/Disable Light Probes for all Renderers")]
    static void Execute()
    {
        var renderers = GameObject.FindObjectsOfType<Renderer>();

        if (renderers.Length <= 0)
            return;

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
}