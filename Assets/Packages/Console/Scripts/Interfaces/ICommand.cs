using UnityEngine;

namespace VARConsole
{
    public interface ICommand
    {
        public string Prefix { get; }

        public string Description { get; }

        public void RunCommand(OnCommandEventArgs e);
    }
}
