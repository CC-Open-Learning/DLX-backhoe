/*
 * SUMMARY: This file contains the Horn class. The purpose this class is to allow the horn to
 *          be used and heard.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Audio;
using RemoteEducation.Interactions;
using RemoteEducation.Scenarios.Inspectable;

public class Horn : SemiSelectable, IInitializable, IBreakable
{
    [Tooltip("The sound the horn makes when pressed")]
    [SerializeField] private SoundEffect HornSound;

    /// <summary> Prevents the horn sound playing if it has been set to broken.</summary>
    private bool hornBroken;

    public void AttachInspectable(DynamicInspectableElement inspectableElement, bool broken, int breakMode)
    {
        hornBroken = broken;

        inspectableElement.OnCurrentlyInspectableStateChanged += (active) =>
        {
            ToggleFlags(!active, Flags.Highlightable | Flags.ExclusiveInteraction);
        };
    }

    public void Initialize(object input = null)
    {
        OnSelect += PlayHornSound;
    }

    /// <summary>Plays the horn sound if the horn is not set to be broken.</summary>
    private void PlayHornSound()
    {
        if (hornBroken)
            return;

        HornSound.Play();
    }
}
