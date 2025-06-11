using Lean.Gui;
using System.Collections.Generic;
using UnityEngine;

namespace RemoteEducation.Scenarios
{
    public class ResultCanvas : MonoBehaviour
    {
        public ScenarioResult ScenarioResult;
        public LeanButton CompleteButton;

        public void ShowResultCanvas(Scenario scenario, List<TaskDetails> details)
        {
            gameObject.SetActive(true);
            ScenarioResult.LoadResults(scenario, details);
        }
    }
}