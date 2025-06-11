using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace RemoteEducation.Editor.BugReporting
{
    public interface ITicketPlatform
    {
        public Task<HttpResponseMessage> SendBugReport(BugReportData payload);
        public bool HasCredentials();
        public void SetCredentials(string username, string password);
        public (string user, string pass) GetCredentials();
    }
}
