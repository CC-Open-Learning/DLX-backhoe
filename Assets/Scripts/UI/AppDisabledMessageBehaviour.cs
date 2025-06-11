/*
 *  FILE    : AppDisabledMessageBehaviour
 *  PROJECT : CORE Engine
 *  AUTHOR  : David Inglis
 *  DATE    : 2020-09-23
 *  DESC    :
 *      Updates a splash screen with a message that the current version of 
 *      the CORE Engine application is disabled. Clicking on the text will
 *      launch a 'mailto' URL with the Conestoga VARLab organization email
 *      address included
 */

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using RemoteEducation.Localization;

namespace RemoteEducation.UI
{
    /// <summary>
    ///     Updates a TextMeshProUGUI component with a message that the current version of 
    ///     the CORE Engine application is disabled.
    ///     <para/>
    ///     Clicking on the text will launch a <c>mailto</c> URL with the Conestoga VARLab 
    ///     organization email address included.
    /// </summary>
    public class AppDisabledMessageBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField]
        private Color colorStandard = Color.white;

        [SerializeField]
        private Color colorSelected = new Color(0, 149, 255);   // Blue color used elsewhere in CORE Engine project

        [SerializeField]
        private TextMeshProUGUI messageText;

        // VARLab Organization email address
        [SerializeField] private string contactEmail = "varlab@stu.conestogac.on.ca";

        // URI used to launch a mail provider, where the {0} parameter is an
        // email address as the recipient, and the {1} parameter is the CORE
        // Engine application version to be used in the SUBJECT line
        [SerializeField] private string mailtoURI = "mailto:{0}?subject=CORE%20Engine%20v{1}";

        // Message to be displayed to the user when CORE Engine is disabled, where 
        // the {0} parameter is a color in HEX format, and the {1} parameter
        // is an email address
        // This string is localized, see the corresponding language files for the full message
        private readonly string messageToken = "Engine.DisabledMessage";

        

        /*
         *  METHOD      : LaunchMailingLink
         *  DESCRIPTION :
         *      Launches a 'mailto' URI with the VARLab organization email address
         *      as the "TO" field
         *  PARAMETERS  :
         *  RETURNS     :
         */
        private void LaunchMailingLink()
        {
            // The Application.OpenURL() call is very powerful, so it should be ensured that no
            // user-crafted string is passed into said function
            Application.OpenURL(string.Format(mailtoURI, contactEmail, Application.version));
        }


        /*  METHOD      : ApplyMessageFormat
         *  DESCRIPTION :
         *      Using the class fields 'formattedMessage' and 'contactEmail',
         *      this method applies a string created with the 'formattedMessage' to
         *      the text of the 'messageText' TextMeshProUGUI class field. Given a
         *      specific Color object, the 'color' will be used in the formatted string
         *  PARAMETERS  :
         *      Color color : The color to be used in the formatted message string
         *  RETURNS     :
         *      void
         */
        private void ApplyMessageFormat(Color color)
        {
            if (messageText)
            {
                messageText.text = string.Format(messageToken.Localize(), ColorUtility.ToHtmlStringRGB(color), contactEmail);
            }
            else
            {
                Debug.LogWarning("AppDisabledMessageBehaviour : No TextMeshProUGUI available to set error message");
            }
        }


        // Set the dynamic message when the component is first instantiated
        public void Awake()
        {
            ApplyMessageFormat(colorStandard);
        }

        // Launch a 'mailto' URI when the user clicks on the message
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                LaunchMailingLink();
            }
        }

        // Update message with 'colorSelected' color on hover
        public void OnPointerEnter(PointerEventData eventData)
        {
            ApplyMessageFormat(colorSelected);
        }

        // Update message with 'colorStandard' color on hover
        public void OnPointerExit(PointerEventData eventData)
        {
            ApplyMessageFormat(colorStandard);
        }
    }
}