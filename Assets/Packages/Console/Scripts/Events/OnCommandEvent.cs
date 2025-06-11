namespace VARConsole
{
    public class OnCommandEventArgs
    {
        public bool HideMessage;
        public string Message;
        public string Command;

        public OnCommandEventArgs(string command)
        {
            HideMessage = false;
            Command = command;
        }

        public bool IsThisCommand(string command)
        {
            var startsWith = Command.StartsWith(command + " "); // Make sure the command has a space after it and before the value.
            var isExact = Command.Trim().Equals(command); // If using a command with no value, trim the spaces off and check if they're equal.

            return startsWith || isExact; 
        }

        public bool TryGetValue<T>(out T value, int startIndex, string errorMessage)
        {
            if (!Command.TrySanitizeCommandValue(startIndex + 1, out value))
            {
                Message = errorMessage;
                return false;
            }

            return true;
        }
    }

    public delegate void OnCommandEvent(OnCommandEventArgs e);
}