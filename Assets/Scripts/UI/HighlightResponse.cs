/*
 *  FILE          :	HighlightResponse.cs
 *  PROJECT       :	VARLab CORE Template
 *  PROGRAMMER    :	Chowon Jung
 *  FIRST VERSION :	2020-08-18
 *  DESCRIPTION   : This file contains the HighlightResponse class which is responsible for displaying
 *                  correct and incorrect response effects on the requested position.
 */

#region Using Statements

using System.Collections;
using UnityEngine;

#endregion

namespace RemoteEducation.UI
{
    public class HighlightResponse : MonoBehaviour
    {
        [Tooltip("Correct Response Effect Prefab")]
        [SerializeField]
        public GameObject CorrectEffect;

        [Tooltip("Correct Response Effect Duration")]
        [SerializeField]
        public float CorrectEffectTime;

        [Tooltip("Incorrect Response Effect Prefab")]
        [SerializeField]
        public GameObject IncorrectEffect;

        [Tooltip("Incorrect Response Effect Duration")]
        [SerializeField]
        public float IncorrectEffectTime;


        private GameObject correctCircle;       // spawned correct effect object
        private GameObject incorrectCircle;     // spawned incorrect effect object


        // Start is called before the first frame update
        void Start()
        {
            // set up effect prefabs
            if (CorrectEffect == null)
            {
                CorrectEffect = Resources.Load("HighlightResponse/Prefabs/Light_Green") as GameObject;
            }
            if (IncorrectEffect == null)
            {
                IncorrectEffect = Resources.Load("HighlightResponse/Prefabs/Light_Red") as GameObject;
            }

            // set up initial position and effect play time
            if (CorrectEffect != null)
            {
                CorrectEffectTime = 1.1f;
                correctCircle = Instantiate(CorrectEffect, new Vector3(0, -2, 0), Quaternion.identity);
            }
            if (IncorrectEffect != null)
            {
                IncorrectEffectTime = 0.6f;
                incorrectCircle = Instantiate(IncorrectEffect, new Vector3(0, -2, 0), Quaternion.identity);
            }

            if (CorrectEffect == null || IncorrectEffect == null)
            {
                Debug.LogError("HighlightObject : Missing effect component to highlight.");
            }

            // test
            // StartCoroutine(Correct(new Vector3(0, 0, 0)));
            // StartCoroutine(Incorrect(new Vector3(0, 1, 0)));
        }



        /*
        * FUNCTION    : Incorrect()
        * DESCRIPTION : This method will play highlight effect on the passed in position when called.
        * PARAMETERS:
        *		void
        * RETURNS     :
        *		void
        */
        public IEnumerator Incorrect(Vector3 pos)
        {
            if (IncorrectEffect != null)
            {
                // move the effect object on the requested position
                incorrectCircle.transform.position = pos;

                // enable for given time and disable back
                incorrectCircle.SetActive(true);
                yield return new WaitForSeconds(IncorrectEffectTime);
                incorrectCircle.SetActive(false);
            }
            else
            {
                Debug.LogError("HighlightObject : Missing effect component to highlight.");
            }
        }



        /*
        * FUNCTION    : Correct()
        * DESCRIPTION :  This method will play highlight effect on the passed in position when called.
        * PARAMETERS:
        *		void
        * RETURNS     :
        *		void
        */
        public IEnumerator Correct(Vector3 pos)
        {
            if (CorrectEffect != null)
            {
                // move the effect object on the requested position
                correctCircle.transform.position = pos;

                // enable for given time and disable back
                correctCircle.SetActive(true);
                yield return new WaitForSeconds(CorrectEffectTime);
                correctCircle.SetActive(false);
            }
            else
            {
                Debug.LogError("HighlightObject : Missing effect component to highlight.");
            }
        }
    }
}