using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Status = RemoteEducation.Scenarios.Inspectable.InspectableElement.Status;

public class StatusIconController : MonoBehaviour
{

    [SerializeField]
    private GameObject checkIcon;

    [SerializeField]
    private GameObject xIcon;

    [SerializeField]
    private GameObject thumbUpIcon;

    [SerializeField]
    private GameObject thumbsDownIcon;

    [SerializeField]
    private GameObject exclamationIcon;

    public enum IconTypes
    {
        blank,
        check,
        x,
        thumbsUp,
        thumbsDown,
        exclamation
    }

    public void SetIcon(Status icon)
    {
        checkIcon.SetActive(icon == Status.EvaluatedCorrect);
        xIcon.SetActive(icon == Status.EvaluatedIncorrect);
        thumbUpIcon.SetActive(icon == Status.InspectedPositive);
        thumbsDownIcon.SetActive(icon == Status.InspectedNegitive);
        exclamationIcon.SetActive(icon == Status.Warning);
    }
}
