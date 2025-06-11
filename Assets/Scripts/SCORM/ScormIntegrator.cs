using System.Collections;
using UnityEngine;
using VARLab.Analytics;
using VARLab.SCORM;

namespace RemoteEducation.Networking.SCORM
{
    public delegate void OnInitializeEvent();

    public class ScormIntegrator : MonoBehaviour
    {
        //[SerializeField]
        //private COREAnalytics _loginObjAnalytics;

        public static ScormIntegrator instance;

        public static event OnInitializeEvent OnInitialize;
        public static string LearnerId { get; private set; }
        public static bool Initialized { get; private set; } = false;

        private void Start()
        {
            instance = this;
            LearnerId = "TestingInEditor";
        }

        private void Awake()
        {
            //ScormManager.OnScormMesssage += ScormManager_OnScormMessage; // The Old manager uses a delegate solution these are commented out for now to get rid of errors.

            // Just in case we somehow load this after the ScormManager has already had a chance to be initialized.
            if (ScormManager.Initialized)
                StartCoroutine(Startup());
        }

        private void OnDestroy()
        {
           //ScormManager.OnScormMessage -= ScormManager_OnScormMessage;
        }

        public void ManagerInitialized()
        {
            StartCoroutine(Startup());
        }

        private void ScormManager_OnScormMessage(ScormManager.Event e)
        {
            switch (e)
            {
                case ScormManager.Event.Initialized:
                    StartCoroutine(Startup());
                    break;

                case ScormManager.Event.Commit:
                    ScormManager.Terminate();
                    break;
            }
        }

        public IEnumerator Startup()
        {
            yield return new WaitForSeconds(2); // Race condition, should be a callback or a timeout-based wait until.
            InitializeData();
        }

        private void InitializeData()
        {
            // Must be set to incomplete at the start so that the LMS does not set to complete as soom as the student opens the Module
            ScormManager.SetCompletionStatus(StudentRecord.CompletionStatusType.incomplete);
#if UNITY_EDITOR
            //LearnerId = "TestingInEditor";
#endif
#if !UNITY_EDITOR
            //Analytics Login now we have ID
            LearnerId = ScormManager.GetLearnerId();
            Debug.Log($"eConestoga Learner ID: {LearnerId}");
            _loginObjAnalytics.LoginLearner( LearnerId );
#endif
            Initialized = true;
            ScormIntegrator.OnInitialize?.Invoke();
        }

        public static void Completed()
        {
            if (instance != null)
                instance._Completed();
        }

        private void _Completed()
        {
            ScormManager.SetCompletionStatus(StudentRecord.CompletionStatusType.completed);
        }

        public static bool IsCompleted()
        {
            if (instance == null)
                return false;

            return ScormManager.GetCompletionStatus() == StudentRecord.CompletionStatusType.completed;
        }
    }
}