/*
 *  FILE          :	FaceThePlayer.cs
 *  PROJECT       :	JWCProject
 *  PROGRAMMER    :	Michael Hilts
 *  FIRST VERSION :	2019-07-05
 *  DESCRIPTION   : This file contains the FaceThePlayer class which does exactly as the
 *                  name suggests.
 */

#region Using Statements

using UnityEngine;

#endregion

namespace RemoteEducation.Helpers.Unity
{
    /*
     * CLASS:       FaceThePlayer
     * DESCRIPTION: This class is responsible for making whatever game object it is attached
     *              to face the player.
     */

    public class FaceThePlayer : MonoBehaviour
    {
        private Transform player;   // The players transform to face

        #region MonoBehaviour Callbacks

        /*
         * FUNCTION    : Start()
         * DESCRIPTION : Start is called once before the first frame update. It will
         *               find the reference to the player.
         * PARAMETERS  :
         *		VOID
         * RETURNS     :
         *		VOID
         */

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("MainCamera").transform;

            if (player == null)
            {
                Debug.LogError("No gameObject with the tag player found in the current scene. Destroying LookAtPlayer component.");
                Destroy(this);
            }
        }

        /*
         * FUNCTION    : Update()
         * DESCRIPTION : Update is called once per frame. This will cause the gameobject for
         *               whom this script is attached to face the player.
         * PARAMETERS  :
         *		VOID
         * RETURNS     :
         *		VOID
         */

        void Update()
        {
            transform.LookAt(player);
        }

        #endregion
    }
}