/*
 *  FILE          :	APIGrades.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	Jacob Nelson
 *  FIRST VERSION :	2020-11-25
 *  DESCRIPTION   : 
 */


using RestSharp;
using System.Net;
using System;
using UnityEngine;

namespace RemoteEducation.Networking.LTI
{

    public static class APIGrades
    {

        private const string url = "https://core-web-var.herokuapp.com/sendGrade";
        private const string CourseIdKey = "courseid";
        private const string UserIdKey = "userid";
        private const string GradeIdKey = "gradeid";



        public static async void SendTestGrade(float grade, Action<bool> callback = null)
        {

            if (!(CommandLineProcessing.GetArgument(CourseIdKey, out string courseId)
                && CommandLineProcessing.GetArgument(UserIdKey, out string userId)
                && CommandLineProcessing.GetArgument(GradeIdKey, out string gradeId)))
            {
                callback?.Invoke(false);
                return;
            }


            Debug.Log("APIGrades : Attempting to send API grade...");
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.POST)
            {
                RequestFormat = DataFormat.Json
            };

            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json",
                '{' +
                "\"orgUnit\": " + courseId + "," +
                "\"userId\": " + userId + "," +
                "\"gradeId\": " + gradeId + "," +
                "\"grade\": \"" + grade + "\"" +
                "}", ParameterType.RequestBody);

            IRestResponse response = await client.ExecutePostAsync<IRestResponse>(request);
            Debug.Log(string.Format("APIGrades : Response \"{0}\"", response.Content));

            bool retval = response.StatusCode == HttpStatusCode.OK;

            callback?.Invoke(retval);
        }
    }
}