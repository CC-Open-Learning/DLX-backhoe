/*
* FILE			: Lerp.cs
* PROJECT		: JWCProject
* PROGRAMMER	: Kieron Higgs
* FIRST VERSION : Dec. 5th, 2019
* LAST UPDATE   : Dec. 5th, 2019
*
*      This mono-method class is used to animate objects in the JWC simulation when they are "dropped" and must be placed in a certain
*      position and rotation.
* 
*/

using System;
using System.Collections;
using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
    /*
     * CLASS:       Lerp
     * DESCRIPTION: This class contains a single method used to animate an object, "flying" it from point A to point B.
     */
    public class Lerp : MonoBehaviour
    {
        /*
         * FUNCTION    : LerpObject()
         * DESCRIPTION : Moves an object smoothly frame-by-frame from its original position and rotation to a new position and rotation.
         * PARAMETERS  :
         *      Transform objectT, Transform destinationT
         *      float startTime, float duration
         *		IEnumerator Finish
         * RETURNS     :
         *		IEnumerator - This is a coroutine, so it requires an IEnumerator return value to be sent via 'yield' return
         */
        public static IEnumerator LerpObject(Transform objectT, Transform destinationT,
                                             float startTime, float duration,
                                             IEnumerator Finish = null)
        {
            bool playerTeleported = false;
            Vector3 startPos = Vector3.zero;
            Vector3 endPos = Vector3.zero;
            Quaternion startRot = Quaternion.identity;
            Quaternion endRot = Quaternion.identity;

            try
            {
                startPos = objectT.position;
                startRot = objectT.rotation;
                endPos = destinationT.position;
                endRot = destinationT.rotation;
            }
            catch (NullReferenceException nullException)
            {
                Debug.Log(nullException);
                playerTeleported = true;
            }

            if (!playerTeleported)
            {
                float elapsedTime = 0f;
                while (elapsedTime < duration)
                {
                    if (objectT == null || destinationT == null)
                    {
                        break;
                    }

                    float percentageComplete = elapsedTime / duration;

                    objectT.position = Vector3.Lerp(startPos, endPos, percentageComplete);
                    objectT.rotation = Quaternion.Lerp(startRot, endRot, percentageComplete);

                    elapsedTime = Time.time - startTime;
                    yield return null;
                }
            }

            yield return Finish;
        }
    }
}