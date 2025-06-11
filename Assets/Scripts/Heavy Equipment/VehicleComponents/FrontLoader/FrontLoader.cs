/*
* SUMMARY: Contains the FrontLoader class, initializes the front loader and used to start lifting and locking animations.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Interactions;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public sealed class FrontLoader : MonoBehaviour, IInitializable
    {
        /// <summary>Animation states of the front loader.</summary>
        public enum State
        {
            Idle = 0,
            Lift = 1,
            LiftLockProcedure = 2
        }

        /// <summary>Controls the animator of the front loader.</summary>
        private Animator loaderAnimator;

        /// <summary><see cref="int"/> hash of the State parameter used by the <see cref="Animator"/>.</summary>
        private static readonly int stateAnimHash = Animator.StringToHash("State");
        
        public void Initialize(object input = null)
        {
            loaderAnimator = GetComponent<Animator>();
        }

        /// <summary>Animates the front loader arms.</summary>
        /// <param name="loaderArmState">animation state to be used.</param>
        public void StartLoader(State loaderArmState)
        {
            loaderAnimator.SetInteger(stateAnimHash, (int) loaderArmState);
        }
    }
}