/*
 *  FILE          :	ExtendScript.cs
 *  PROJECT       :	JWCProject
 *  PROGRAMMER    :	Ivan Granic
 *  FIRST VERSION :	2020-07-29
 *  DESCRIPTION   : Simple functions to set true or false for animator's boolean value. causes animation to fire
 */

using UnityEngine;

namespace RemoteEducation.Helpers.Unity
{
    public class ExtendScript : MonoBehaviour
    {
        public GameObject panelmenu;
        /*
         * FUNCTION    : EditExtend()
         * DESCRIPTION : Simple function to set animator boolean to true when called by On Value Changed(); in order 
         *               for extending animation to fire
         * PARAMETERS  :
         *		VOID
         * RETURNS     :
         *		VOID
         */
        public void EditExtend()
        {
            if (panelmenu != null)
            {
                Animator animator = panelmenu.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("extend", true);
                }
            }
        }
        /*
         * FUNCTION    : EditCollapse()
         * DESCRIPTION : Simple function to set animator boolean to false when called by On End Edit(); in order 
         *               for collapsing animation to fire
         * PARAMETERS  :
         *		VOID
         * RETURNS     :
         *		VOID
         */
        public void EditCollapse()
        {
            if (panelmenu != null)
            {
                Animator animator = panelmenu.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetBool("extend", false);
                }
            }
        }
    }
}