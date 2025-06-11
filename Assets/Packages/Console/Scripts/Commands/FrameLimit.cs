using UnityEngine;

namespace VARConsole.Commands
{
    public class FrameLimit : ICommand
    {
        public string Prefix => "limitfps";

        public string Description => "Sets Application.targetFramerate to the given int value.";

        public void RunCommand(OnCommandEventArgs e)
        {
            if (e.IsThisCommand(Prefix) && e.TryGetValue(out int value, Prefix.Length, Description))
            {
                Application.targetFrameRate = value;
                e.HideMessage = true;
            }
        }
    }
}
