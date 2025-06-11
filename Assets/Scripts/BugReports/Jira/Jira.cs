using Newtonsoft.Json.Linq;
using RemoteEducation.Scenarios;
using RemoteEducation.Scenarios.Inspectable;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RemoteEducation.Editor.BugReporting
{
    /// <summary>Handles communication with the Jira ticketing platform.</summary>
    public partial class Jira : ITicketPlatform
    {
        private const string JIRA_EMAIL_REGISTRY_KEY = "JIRA_EMAIL";
        private const string JIRA_API_KEY_REGISTRY_KEY = "JIRA_API_KEY";
        private const string API_URL = "https://varlab-dev.atlassian.net/rest/api/3/issue/";
        private const string JIRA_TICKET_URL = "https://varlab-dev.atlassian.net/browse/";

        private const string COMMITS_URL = "https://bitbucket.org/VARLab/core-engine/commits";
        private const string BRANCHES_URL = "https://bitbucket.org/VARLab/core-engine/branch";

        public async Task<HttpResponseMessage> SendBugReport(BugReportData bugReportData)
        {
            bugReportData.Environment = CreateEnvironmentString();
            bugReportData.Description = bugReportData.Description.Replace("\n", "\\n");

            using (var client = new HttpClient())
            {
                SetupClientForJira(client, API_URL);

                var payload = CreateReportPayload(bugReportData);

                var data = new StringContent(payload, Encoding.UTF8, "application/json");
                var responseMessage = await client.PostAsync(API_URL, data);

                var ticketData = await responseMessage.Content.ReadAsStringAsync();

                if(responseMessage.StatusCode != System.Net.HttpStatusCode.Created)
                    return responseMessage;

                var jsonDict = JObject.Parse(ticketData).Properties().ToDictionary(k => k.Name, v => v.Value.ToString());

                var id = jsonDict["id"];
                var ticketNumber = jsonDict["key"];

                if (File.Exists(StateLogger.CurrentStateLog))
                    await AddAttachment(StateLogger.CurrentStateLog, "States", id);

                if (File.Exists(BugReporter.TempConsoleLogFilePath))
                    await AddAttachment(BugReporter.TempConsoleLogFilePath, "Console Log", id);

                Application.OpenURL(JIRA_TICKET_URL + jsonDict["key"]);

                Debug.Log($"Successfully created bug ticket {ticketNumber}.");

                return responseMessage;
            }
        }

        /// <summary>Creates the string used for the Environment field of the Jira ticket.</summary>
        private string CreateEnvironmentString()
        {
            var location = "";

            if (ScenarioManager.Instance != null)
            {
                var scenarioData = ScenarioManager.Instance.CurrentScenario;

                location += $"{scenarioData.Module} - {scenarioData.Title}\\n";
            }
            else
                location += "Main Menu\\n";

            return CreateEnvironmentField(location, Git.GetBranch(), Git.GetCommit());
        }


        private void SetupClientForJira(HttpClient client, string uri)
        {
            client.BaseAddress = new Uri(uri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));   
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetCredentialsBase64());
        }

        public void SetCredentials(string username, string apikey)
        {
            if(!string.IsNullOrEmpty(username))
                PlayerPrefs.SetString(JIRA_EMAIL_REGISTRY_KEY, username);

            if (!string.IsNullOrEmpty(apikey))
                PlayerPrefs.SetString(JIRA_API_KEY_REGISTRY_KEY, apikey);
        }

        private string GetCredentialsBase64()
        {
            var credentials = GetCredentials();
            var credentialString = $"{credentials.user}:{credentials.pass}";
            return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(credentialString));
        }

        public (string user, string pass) GetCredentials()
        {
            return (PlayerPrefs.GetString(JIRA_EMAIL_REGISTRY_KEY), PlayerPrefs.GetString(JIRA_API_KEY_REGISTRY_KEY));
        }

        public bool HasCredentials()
        {
            return PlayerPrefs.HasKey(JIRA_EMAIL_REGISTRY_KEY) && PlayerPrefs.HasKey(JIRA_API_KEY_REGISTRY_KEY);
        }

        public async Task<HttpResponseMessage> AddAttachment(string fileName, string title, string issueKey)
        {
            using var file = File.OpenRead(fileName);

            string url = $"{API_URL}{issueKey}/attachments";
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", GetCredentialsBase64());
            client.DefaultRequestHeaders.Add("X-Atlassian-Token", "no-check");

            MultipartFormDataContent multiPartContent = new MultipartFormDataContent("-data-");

            ByteArrayContent byteArrayContent;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                //string fileString = Convert.ToBase64String(fileBytes);
                byteArrayContent = new ByteArrayContent(fileBytes);
            }

            multiPartContent.Add(byteArrayContent, "file", title);

            var response = await client.PostAsync(url, multiPartContent);

            return response;
        }
    }
}
