using UnityEngine;
using UnityEngine.UI;

namespace RemoteEducation.UserReporting
{
    public class SubmitFeedbackButton : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OpenUserReportingForm);
        }

        public void OpenUserReportingForm()
        {
            UserReportingScript.CreateUserReport();
        }
    }
}