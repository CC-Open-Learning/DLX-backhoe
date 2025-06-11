using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;
using System.Linq;

namespace VARConsole
{
    public static class DefaultCommands
    {
        private const string COMMAND_NAMESPACE = "VARConsole.Commands";

        public static void AddDefaultCommands()
        {
            var commands = GetDefaultCommands();

            for (int i = 0; i < commands.Length; i++)
            {
                VARConsole.AddCommand(commands[i]);
            }
        }

        public static ICommand[] GetDefaultCommands()
        {
            var types = GetTypesInNamespace(Assembly.GetExecutingAssembly(), COMMAND_NAMESPACE);

            var commands = new ICommand[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                commands[i] = (ICommand)Activator.CreateInstance(types[i]);
            }

            return commands;
        }

        private static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            return
              assembly.GetTypes()
                      .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && typeof(ICommand).IsAssignableFrom(t))
                      .ToArray();
        }

        public static bool TrySanitizeCommandValue<T>(this string input, int index, out T value)
        {
            try
            {
                value = (T)Convert.ChangeType(input.Substring(index).Trim(), typeof(T));
                return true;
            }
            catch 
            {
                value = default(T);
                return false;
            }
        }
    }
}