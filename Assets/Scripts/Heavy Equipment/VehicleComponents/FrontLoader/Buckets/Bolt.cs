/*
 * SUMMARY: The purpose of this file is to model the behaviour and properties of a bolt on the front loader bucket.
 *          This includes a bool to track state (tight or loose), tooltips, OnSelect toast messages informing the 
 *          user of the state, and a sound/animation which plays only if the bolt is loose to ensure users know which 
 *          state the bolt is in
 */



using RemoteEducation.Audio;
using RemoteEducation.Scenarios.Inspectable;
using RemoteEducation.Interactions;
using RemoteEducation.Localization;
using RemoteEducation.UI;
using RemoteEducation.UI.Tooltip;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RemoteEducation.Scenarios;



namespace RemoteEducation.Modules.HeavyEquipment
{
    public class Bolt : SemiSelectable
    {
        [SerializeField, Tooltip("Sound effect played when the loose bolt is checked")]
        private SoundEffect looseBoltSoundEffect;

        [SerializeField, Tooltip("Used to animated the bolt rotating away then back.")]
        private AnimationCurve animationCurve;

        /// <summary>Duration of the animation</summary>
        private float animationDuration = 1.2f;

        /// <summary>angle that the bolt will rotate and then rotate back</summary>
        private float angle = 120f;

        /// <summary>Flag to track if the bolt is loose or tight</summary>
        private bool loose = false;

        /// <summary>Flag to track if the bolt is currently rotating</summary>
        private bool isAnimating = false;

        [SerializeField, Tooltip("Used to show if it has been inspected")]
        private bool IsInspected = false;

        private Bucket parent;

        /// <summary>Called by bucket class to Initialize the bolts. Sets tooltips and the OnSelect method to 
        /// display the bolt state in a toast</summary>
        public void Setup(Bucket parentBucket)
        {
            parent = parentBucket;
            SetToolTip();
            OnSelect += CheckBolt;
        }


        /// <summary>Adds a tooltip to the bolt using a string token in the english.txt file</summary>
        private void SetToolTip()
        {
            if (gameObject.GetComponent<SimpleTooltip>() == null)
            {
                SimpleTooltip tooltip = gameObject.AddComponent<SimpleTooltip>();
                tooltip.tooltipText = Localizer.Localize("HeavyEquipment.BucketBoltTooltip");
            }
        }

        /// <summary>OnSelect checks the current bolt state. Always display a toast message telling the user if the bolt 
        /// is tight or loose. Then if it is loose, play a sound and start a coroutine for an animation of the bolt rotating</summary>
        private void CheckBolt()
        {
            DisplayToastMessage();

            if (loose)
            {
                if (!isAnimating)
                {
                    looseBoltSoundEffect.Play();
                    StartCoroutine(RotateLooseBolt());
                }
            }

            parent.PokeBolts();
        }

        /// <summary>Display toast message to user indicating bolt state. (Loose or tight)</summary>
        private void DisplayToastMessage()
        {
            string boltStateToast = loose ? Localizer.Localize("HeavyEquipment.BucketBoltToastLoose") : Localizer.Localize("HeavyEquipment.BucketBoltToastTight");

            HEPrompts.CreateToast(boltStateToast, HEPrompts.ShortToastDuration);

            IsInspected = true;
        }

        /// <summary>Setter for the bucket class to set the bolt loose when randomizing loose bolts if the bucket is
        /// in the loose bolts breakMode</summary>
        public void SetBoltLoose()
        {
            loose = true;
        }

        /// <summary>Animation method for the bolt rotating a little, and then turning back. Mimics the movement
        /// if someone were to loosen a bolt and then tighten it back by hand.</summary>
        /// <returns>null</returns>
        private IEnumerator RotateLooseBolt()
        {
            isAnimating = true;

            var startRotation = transform.localRotation;
            var endRotation = Quaternion.Euler(transform.localEulerAngles + Vector3.up * angle);

            for (var t = 0f; t < 1; t += Time.deltaTime / animationDuration)
            {
                transform.localRotation = Quaternion.Lerp(startRotation, endRotation, animationCurve.Evaluate(t));
                yield return null;
            }

            isAnimating = false;
        }

        public bool CheckTask()
        {
            return (IsInspected) ? true : false;
        }
    }
}