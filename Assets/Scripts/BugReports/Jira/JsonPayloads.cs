using RemoteEducation.Scenarios;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RemoteEducation.Editor.BugReporting
{
    partial class Jira
    {
        private static string CreateReportPayload(BugReportData bugReportData)
        {
            return  "{" +
                        "\"fields\":" +
                        "{" +
                            "\"project\":" +
                            "{" +
                                $"\"key\": \"{bugReportData.ProjectKey}\"" +
                            "}," +
                            $"\"summary\": \"{bugReportData.Summary}\"," +
                            CreateTextField("description", bugReportData.Description) +
                            bugReportData.Environment +
                            "\"issuetype\": " +
                            "{" +
                                $"\"name\": \"Bug\"" +
                            "}" +
                        "}" +
                    "}";
        }

        private static string CreateTextField(string fieldName, string content)
        {
            return $"\"{fieldName}\": " +
                    "{" +
                        "\"type\": \"doc\"," +
                        "\"version\": 1," +
                        "\"content\": [" +
                        "{" +
                            "\"type\": \"paragraph\"," +
                            "\"content\": [ " +
                            "{" +
                                "\"type\": \"text\"," +
                                $"\"text\": \"{content}\"" +
                            "}" +
                            "]" +
                        "}" +
                        "]" +
                    "},";
        }

        private static string CreateEnvironmentField(string location, string branch, string commit)
        {
            return $"\"environment\": " +
                    "{" +
                        "\"type\": \"doc\"," +
                        "\"version\": 1," +
                        "\"content\": [" +
                        "{" +
                            "\"type\": \"paragraph\"," +
                            "\"content\": [ " +
                            "{" +
                                "\"type\": \"text\"," +
                                $"\"text\": \"Scenario: {location}\"" +
                            "}," +
                            "{" +
                                "\"type\": \"text\"," +
                                $"\"text\": \"Branch Link ({branch})\\n\"," +
                                "\"marks\": [" +
                                "{" +
                                    "\"type\": \"link\"," +
                                    "\"attrs\":" +
                                    "{" +
                                        $"\"href\": \"{BRANCHES_URL}/{branch}\"" +
                                    "}" +
                                "}" +
                                "]" +
                            "}," +
                            "{" +
                                "\"type\": \"text\"," +
                                "\"text\": \"Commit Link\"," +
                                "\"marks\": [" +
                                "{" +
                                    "\"type\": \"link\"," +
                                    "\"attrs\":" +
                                    "{" +
                                        $"\"href\": \"{COMMITS_URL}/{commit}\"" +
                                    "}" +
                                "}" +
                                "]" +
                            "}" +
                            "]" +
                        "}" +
                        "]" +
                    "},";
        }
    }
}
