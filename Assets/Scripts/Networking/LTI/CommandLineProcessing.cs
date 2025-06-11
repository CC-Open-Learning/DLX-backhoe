/*
 *  FILE          :	CommandLineProcessing.cs
 *  PROJECT       :	CORE Engine
 *  PROGRAMMER    :	David Inglis, Jacob Nelson
 *  FIRST VERSION :	2020-12-07
 *  DESCRIPTION   : 
 */

using System;
using System.Collections.Generic;

namespace RemoteEducation.Networking.LTI
{
    public static class CommandLineProcessing
    {
        private const char ArgumentSeparator = '&';
        private const char KeyValuePairSeparator = '=';

        private static Dictionary<string, string> arguments;

        static CommandLineProcessing()
        {
            arguments = new Dictionary<string, string>();
#if UNITY_EDITOR
            EditorParseArguments();
#else
            ParseArguments();
#endif
        }

        public static bool GetArgument(string key, out string value)
        {
            return arguments.TryGetValue(key, out value);
        }

        private static void ParseArguments()
        {
            string args = null;

            if (Environment.GetCommandLineArgs().Length > 1)
            {
                args = Environment.GetCommandLineArgs()[1];
            }
            
            if (args == null)
            {
                return;
            }

            foreach (string j in args.Split(ArgumentSeparator))
            {

                string[] pair = j.Split(KeyValuePairSeparator);
                if (pair.Length == 2)
                {
                    arguments.Add(pair[0], pair[1]);
                }
            }
        }


#if UNITY_EDITOR
        private static void EditorParseArguments()
        {
            arguments.Add("courseid", "12310");
            arguments.Add("userid", "7514");
            arguments.Add("gradeid", "272995");
        }
#endif
    }
}