using UnityEngine;

namespace VARConsole.Commands
{
    public class Help : ICommand
    {
        public string Prefix => "help";

        public string Description => "Provides information about all console commands.";

        public void RunCommand(OnCommandEventArgs e)
        {
            if (!e.IsThisCommand(Prefix))
                return;

            VARConsole.ListOptions();
            e.HideMessage = true;
        }
    }
}
