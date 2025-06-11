using System;
using System.Text;
using UnityEngine;



    public static class Git
    {
        public static string GetCommit()
        {
            return GetGitConsoleResponse("git rev-parse HEAD");
        }

        public static string GetBranch()
        {
            return GetGitConsoleResponse("git rev-parse --abbrev-ref HEAD");
        }

        private static string GetGitConsoleResponse(string command)
        {

            try
            {
                var proc = new System.Diagnostics.Process();

                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
                proc.StartInfo.WorkingDirectory = Application.dataPath;
                proc.EnableRaisingEvents = true;
                proc.StartInfo.Arguments = $"/c {command}";
                proc.Start();

                return proc.StandardOutput.ReadLine();

            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString());
                return null;
            }

        }
    }

