using RemoteEducation.Scenarios;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This interface defines that a class can be used to display UI for the Tasks in the Scenario Graphs.
/// </summary>
public interface IScenarioGraphUI
{
    /// <summary>
    /// This method shall be called after the <see cref="TaskManager"/> has set up all the tasks.
    /// In this method, the class shall attach to all the relevant UI events on the <see cref="TaskManager"/>.
    /// </summary>
    /// <param name="taskManager">The TaskManager in the scene.</param>
    public void InitializeUI(TaskManager taskManager);
}
