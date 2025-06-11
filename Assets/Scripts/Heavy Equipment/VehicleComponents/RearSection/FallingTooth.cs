using RemoteEducation.Interactions;
using System.Collections;
using UnityEngine;
using RemoteEducation.UI;
using RemoteEducation.Localization;

namespace RemoteEducation.Modules.HeavyEquipment
{
    public class FallingTooth : SemiSelectable
    {
        private bool isLoose = false;

        [Tooltip("How quickly the tooth falls down.")]
        public float MovementTime = 3f;

        public bool isInspected = false;

        BoomBucket ParentObject = null;

        public void ToggleLoose()
        {
            isLoose = !isLoose;
        }

        public void Setup(BoomBucket parent)
        {
            OnSelect += CheckLooseTeeth;
            ParentObject = parent;
        }

        /// <summary>function to start the coroutine for loose tooth animation</summary>
        private void CheckLooseTeeth()
        {
            isInspected = true;
            if (isLoose)
            {
                HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.FallingToothToast"), HEPrompts.ShortToastDuration);
                StartCoroutine(DropLooseTooth());
            }
            else
            {
                HEPrompts.CreateToast(Localizer.Localize("HeavyEquipment.GoodToothToast"), HEPrompts.ShortToastDuration);
            }

            ParentObject.PokeTeeth();
        }

        /// <summary>coroutine for loose tooth animation</summary>
        private IEnumerator DropLooseTooth()
        {
            //Start timer and end timer for our animation
            float startTimer = Time.time;
            float endTimer = startTimer + MovementTime;

            Vector3 startingPosition = transform.position;
            Vector3 endingPosition = startingPosition;
            endingPosition.y = 0;

            //While we are animating, animate the object
            while (Time.time <= endTimer)
            {
                float t = (Time.time - startTimer) / MovementTime;

                transform.position = Vector3.Lerp(startingPosition, endingPosition, t);

                yield return null;
            }

            transform.position = startingPosition;
            gameObject.SetActive(false);
        }
    }
}
