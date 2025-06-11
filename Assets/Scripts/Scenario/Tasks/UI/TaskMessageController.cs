using RemoteEducation.Scenarios;
using RemoteEducation.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Show the Task Description of the current task in a scene.
/// To include this functionality in a scene, place the "Task Messages" 
/// prefab into the scene. Then reference the prefab in the module 
/// under <see cref="IExtensionModule.ScenarioGraphUIs"/>.
/// </summary>
public class TaskMessageController : MonoBehaviour, IScenarioGraphUI
{
    private PromptManager promptManager => PromptManager.Instance;
    private int TaskPromptKey;

    /// <summary>After a task is completed, the status icon will update. The message will then
    /// disappear after this time has passes.</summary>
    public float ShowTimeAfterCompletion { get; set; } = 1f;
    
    /// <summary>After the last message has disappeared, wait this long before showing the next prompt.</summary>
    public float TimeBetweenPrompts { get; set; } = 1f;

    /// <summary>
    /// Get key from the <see cref="RemoteEducation.UI.PromptManager"/>.
    /// Add listeners to the events on the <see cref="TaskManager"/>
    /// </summary>
    /// <param name="taskManager">The TaskManager in the scene.</param>
    public void InitializeUI(TaskManager taskManager)
    {
        TaskPromptKey = promptManager.Create(string.Empty);

        taskManager.OnCurrentTaskUpdated += task => AnimateTaskMessage(task as UserTaskVertex);
        taskManager.OnLastTaskCompleted += () => promptManager.HideAll();

        ScenarioManager.Instance.ScenarioEnded.AddListener(() => promptManager.Hide(TaskPromptKey));
    }

    /// <summary>
    /// Call the co-routine <see cref="TaskMessageCoroutine(int, UserTaskVertex)"/>
    /// </summary>
    /// <param name="task">The new task to display</param>
    public void AnimateTaskMessage(UserTaskVertex task)
    {
        StartCoroutine(TaskMessageCoroutine(TaskPromptKey, task));
    }

    /// <summary>
    /// Animate the prompt that is showing the current task description.
    /// It will hide the old task, then show the new one.
    /// </summary>
    /// <param name="promptKey">The key that the task manager uses to interface with the <see cref="PromptManager"/></param>
    /// <param name="task">The task to be displayed</param>
    /// <returns>IEnumerator for the co-routine</returns>
    private IEnumerator TaskMessageCoroutine(int promptKey, UserTaskVertex task)
    {
        if (promptKey == PromptManager.InvalidKey)
        {
            Debug.LogError("PromptManager : Attempted animation on null Prompt");
            yield break;
        }

        //mark current as complete
        promptManager.SetStatus(promptKey, PromptManager.Status.Positive);
        yield return new WaitForSeconds(ShowTimeAfterCompletion);

        //hide current
        promptManager.Hide(promptKey);
        yield return new WaitForSeconds(TimeBetweenPrompts);

        //if there is another prompt to show
        if (task != null && !task.BlankTitleAndDescription)
        {
            //set the status icon and message, then show the prompt
            promptManager.SetStatic(promptKey);
            promptManager.SetStatus(promptKey, PromptManager.Status.Info);
            promptManager.SetMessage(promptKey, task.Description);
            promptManager.Show(promptKey);
        }
    }
}
