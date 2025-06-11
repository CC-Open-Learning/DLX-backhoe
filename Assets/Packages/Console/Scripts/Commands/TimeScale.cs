using UnityEngine;

namespace VARConsole.Commands
{
    public class TimeScale : ICommand
    {
        public string Prefix => "time";

        public string Description => "Sets Time.timeScale to the given float value.";

        public void RunCommand(OnCommandEventArgs e)
        {
            if (e.IsThisCommand(Prefix) && e.TryGetValue(out float value, Prefix.Length, Description))
            {
                Time.timeScale = value;
                e.HideMessage = true;
            }
        }
    }
}
