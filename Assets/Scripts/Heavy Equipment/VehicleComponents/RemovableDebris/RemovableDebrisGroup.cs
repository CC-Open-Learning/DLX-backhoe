/*
 *  FILE            :   RemovableDebrisGroup.cs
 *  PROJECT         :   Heavy Equipment
 *  PROGRAMMER      :   Balazs Karner and Duane Cressman
 *  FIRST VERSION   :   2021-06-15
 *  DESCRIPTION     :   The purpose of this class is to model the behaviour of Removable Debris Groups (Clusters) around 
 *                      the backhoe machine. This includes initializing all debris within the group that should be active,
 *                      keeping track of whether or not they have been cleared, and later on advancing the "clear debris" task
 *                      by toggling debris intractability
 */

using RemoteEducation.Interactions;
using RemoteEducation.Modules.HeavyEquipment;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>This class initializes all individual debris within the cluster, keeps track of when all have been cleared,
/// and will later control advancing through the "clear debris" task.</summary>
public class RemovableDebrisGroup : MonoBehaviour, IBreakable, IInitializable, ITaskable
{
    private const int MINIMUM_DEBRIS_COUNT = 2;
    private const int MAXIMUM_DEBRIS_COUNT = 3;

    public enum DebrisLocations
    {
        PadAndGrouser,
        BucketLinkage,
        SwingLinkage,
        LoaderBucket,
        EngineDebris,
        EngineFireHazard,
        InsideLoaderBucket
    }

    /// <summary>List of individual RemovableDebris within this group (cluster).</summary>
    private List<RemovableDebris> debrisList;
    // Created for UT purpose
    public List<RemovableDebris> DebrisList => debrisList;

    /// <summary>If all debris in this group have been cleared.</summary>
    public bool AllCleared { get; private set; } = false;

    public TaskableObject Taskable { get; private set; }

    [Tooltip("How quickly the debris moves off screen when removed.")]
    public float MovementTime = 3;

    [SerializeField, Tooltip("The location of this debris group.")]
    private DebrisLocations location;
    public DebrisLocations Location => location;

    /// <summary>Initialize each individual debris in the group (cluster).</summary>
    /// <param name="input"><see cref="BackhoeController"/> instance argument.</param>
    public void Initialize(object input = null)
    {
        debrisList.ForEach(x => x.Setup(this));

        Taskable = new TaskableObject(this, location.ToString());
    }

    public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
    {
        GetDebris();

        AllCleared = !broken;

        RandomizeVisableDebris(broken);
    }

    /// <summary>Set up which <see cref="RemovableDebris"/> objects are visible.</summary>
    /// <param name="showDebris">If any debris should be shown</param>
    private void RandomizeVisableDebris(bool showDebris)
    {
        if (!showDebris)
        {
            foreach (RemovableDebris debris in debrisList)
            {
                debris.IsCleared = true;
                debris.gameObject.SetActive(false);
            }

            return;
        }

        if (debrisList.Count < MINIMUM_DEBRIS_COUNT)
        {
            //make all debris visible if there is less than the minimum debris count
            debrisList.ForEach(x => x.gameObject.SetActive(true));
            return;
        }

        var hiddenIndexes = new List<int>();
        for (int i = 0; i < debrisList.Count; i++)
        {
            hiddenIndexes.Add(i);
        }

        // Random.Range(a, b) generates a random number between a and b-1
        var visibleCount = Random.Range(MINIMUM_DEBRIS_COUNT - 1, MAXIMUM_DEBRIS_COUNT + 1);

        //remove the indexes that should be shown
        for (int i = 0; i < visibleCount; i++)
        {
            hiddenIndexes.RemoveAt(Random.Range(0, hiddenIndexes.Count - 1));
        }

        //show all the debris that is not in the hidden list
        for (int i = 0; i < debrisList.Count; i++)
        {
            bool isShown = !hiddenIndexes.Contains(i);

            debrisList[i].IsCleared = !isShown;
            debrisList[i].gameObject.SetActive(isShown);
        }
    }

    /// <summary>Get the list of all children (debris) in the cluster.</summary>
    private void GetDebris()
    {
        debrisList = GetComponentsInChildren<RemovableDebris>(true).ToList();
    }

    /// <summary>
    ///     If a debris was cleared, then this method is called to iterate over the list of debris in the group.
    ///     It checks to see if all debris have been cleared or not. If they have thenAllCleared will be set to true.
    ///     Future plans to control advancing "clear debris" task here
    /// </summary>
    public void DebrisCleared()
    {
        if (!AllCleared)
        {
            bool debrisStillActive = false;

            foreach (RemovableDebris debris in debrisList)
            {
                if (!debris.IsCleared)
                {
                    debrisStillActive = true;
                    break;
                }
            }

            if (!debrisStillActive)
            {
                AllCleared = true;
                Taskable.PokeTaskManager();

                Debug.Log("This area is all cleared!");
            }
        }
    }

    /// <summary>This method disables intractability for every individual debris chunk using<see cref="RemovableDebris.ToggleInteractability(bool)"/></summary>
    /// <param name="isInteractable">A bool whether or not the debris should be interactable or not</param>
    public void ToggleGroupInteractability(bool isInteractable)
    {
        foreach (RemovableDebris debris in debrisList)
        {
            debris.ToggleInteractability(isInteractable);
        }
    }

    public object CheckTask(int checkType, object inputData)
    {
        switch (checkType)
        {
            case (int)TaskVertexManager.CheckTypes.Bool:
                return AllCleared;

            default:
                Debug.LogError($"Invalid check type passed into {GetType().Name}");
                break;
        }

        return null;
    }
}