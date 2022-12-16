namespace Points.Commands
{
    using System;

    using CommandSystem;

    using Internal;

    internal sealed class Save : ICommand
    {
        private Save()
        {
        }

        public static Save Instance { get; } = new Save();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = PointEditor.Save()
                ? "Save Successful"
                : "Load/Create a list first!";

            return true;
        }

        public string Command { get; } = "save";
        public string[] Aliases { get; } = { "s" };
        public string Description { get; } = "Saves the currently loaded file.\n";
    }
}
