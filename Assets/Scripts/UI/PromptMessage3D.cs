/*
 *  FILE          :	PromptMessage3D.cs
 *  PROJECT       :	VARLab CORE Template
 *  PROGRAMMER    :	Kieron Higgs
 *  FIRST VERSION :	2020-08-07
 *  DESCRIPTION   : This file contains the PromptMessage3D class which is responsible for displaying and hiding
 *					3D prompt message on the screen when it is called.
 */


#region Using Statements

using UnityEngine;
using TMPro;

#endregion

namespace RemoteEducation.UI
{
    public class PromptMessage3D : Prompt
    {
        [Tooltip("the TextMeshPro element containing the message")]
        [SerializeField]
        private TextMeshPro messageTMP;

        /*
        * FUNCTION    : SetMessage()
        * DESCRIPTION : This method will assign string message passed in on the
        *				PromptMessage3D window.
        * PARAMETERS:
        *		string message  : the message to display on the 3D prompt window
        * RETURNS     :
        *		void
        */
        public void SetMessage(string message)
        {
            if (messageTMP == null)
            {
                if (transform.Find("Text").GetComponent<TextMeshPro>() != null)
                {
                    messageTMP = transform.Find("Text").GetComponent<TextMeshPro>();
                }
            }

            messageTMP.text = message;
        }
    }
}
