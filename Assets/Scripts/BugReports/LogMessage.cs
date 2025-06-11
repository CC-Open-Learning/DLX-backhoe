using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace RemoteEducation.Editor.BugReporting
{
    public struct LogMessage
    {
        public string condition;
        public string stackTrace;
        public LogType logType;

        public LogMessage(string condition, string stackTrace, LogType logType)
        {
            this.condition = condition;
            this.stackTrace = stackTrace;
            this.logType = logType;
        }
    }
}
