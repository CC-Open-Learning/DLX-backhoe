/*
 *  FILE            :   RemovableDebris.cs
 *  PROJECT         :   Heavy Equipment
 *  PROGRAMMER      :   Balazs Karner and Duane Cressman
 *  FIRST VERSION   :   2021-06-15
 *  DESCRIPTION     :   The purpose of this class is to model the behaviour of RemovableDebris around the backhoe machine. 
 *                      This includes the ability to clear debris OnSelect through an animation done with coroutines, and 
 *                      toggle the interactability of debris within the cluster. Apply this to any debris that needs to be 
 *                      animated. 
 */

using RemoteEducation.Interactions;
using System.Collections;
using UnityEngine;

/// <summary>This class defines the basic functionality of the removable debris found around the backhoe.</summary>
/// <remarks>
///     All removable debris can be removed, but not every debris needs to be sent to the same target.
///     Use Unity inspector to assign a target to every debris int he cluster.
/// </remarks>
public sealed class RemovableDebris : SemiSelectable
{
    /// <summary>A bool which tracks if the rock has been cleared (Moved) yet or not</summary>
    [HideInInspector] public bool IsCleared;

    /// <summary>The <see cref="RemovableDebrisGroup"/> this object is a child of.</summary>
    private RemovableDebrisGroup parentGroup;

    [SerializeField, Tooltip("The end position of the debris when cleared")]
    private Transform target;

    /// <summary>Sets up this debris and assigns its parent <see cref="RemovableDebrisGroup"/>.</summary>
    /// <param name="debrisGroup">The parent debris group.</param>
    public void Setup(RemovableDebrisGroup debrisGroup)
    {
        parentGroup = debrisGroup;
        OnSelect += RemoveDebris;

        RemoveFlags(Flags.ExclusiveInteraction);
    }

    /// <summary>Starts a coroutine to remove the selected debris if it is still present (isCleared == false).</summary>
    private void RemoveDebris()
    {
        if (!IsCleared)
        {
            //Start animation and notify the cluster that this individual debris is cleared
            IsCleared = true;
            parentGroup.DebrisCleared();
            StartCoroutine(DebrisAnimation());
        }
    }

    /// <summary>Creates an animation of debris flying away.</summary>
    /// <remarks>Disables this <see cref="GameObject"/> upon completion.</remarks>
    private IEnumerator DebrisAnimation()
    {
        //Start timer and end timer for our animation
        float startTimer = Time.time;
        float endTimer = startTimer + parentGroup.MovementTime;

        Vector3 startingPosition = transform.position;

        //While we are animating, animate the object
        while (Time.time <= endTimer)
        {
            float t = (Time.time - startTimer) / parentGroup.MovementTime;

            transform.position = Vector3.Lerp(startingPosition, target.transform.position, t);

            yield return null;
        }

        transform.position = startingPosition;
        gameObject.SetActive(false);   
    }

    /// <summary>This method toggles whether or not the debris should be interactable.</summary>
    /// <param name="isInteractable">A bool whether or not the debris should be interactable or not</param>
    public void ToggleInteractability(bool isInteractable)
    {
        ToggleFlags(isInteractable, Flags.InteractionsDisabled);
    }
}