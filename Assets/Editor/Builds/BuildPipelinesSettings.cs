using UnityEditor;
using UnityEngine;
using System;
using VARLab.SCORM;

namespace RemoteEducation.Builds
{
    public class BuildPipelinesSettings : ScriptableObject
    {
        public bool LoadFromCCD;
        public bool BuildAddressables;
        public bool DevelopmentBuild;
        public BuildTarget BuildTarget;
        public bool CreateSCORMPackage;
        public bool RunAfterBuilding;
        public bool UploadToCCD;

        public bool loadSpecificScenario;

        public SCORMExportData SCORMExportData;
    }

    //[Serializable]
    //public class SCORMExportData
    //{
    //    public string ManifestIdentifier;
    //    public string CourseTitle;
    //    public string CourseDescription;
    //    public string SCOTitle;
    //    public string DataFromLMS;
    //    public bool CompletedByProgressAmount;
    //    public float ProgressAmountForCompletion;
    //    public TimeLimitAction TimeLimitAction;
    //    public string TimeLimit;
    //}

    //public enum TimeLimitAction
    //{
    //    None = 0,
    //    ExitWithMessage = 1,
    //    ExitNoMessage = 2,
    //    ContinueWithMessage = 3,
    //    ContinueNoMessage = 4
    //}
}
