namespace Points.Commands
{
    using System;
    using CommandSystem;
    using Internal;

    internal sealed class Load : ICommand
    {
        private Load()
        {
        }

        public static Load Instance { get; } = new Load();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var file = arguments.At(0);
            response = PointEditor.LoadPoints(file)
                ? $"{file} loaded successfully with {PointEditor.CurrentLoadedPointList.RawPoints.Count} point(s)."
                : $"{file} does not exist, created new point list.";

            return true;
        }

        public string Command { get; } = "load";
        public string[] Aliases { get; } = {"l"};
        public string Description { get; } = "Loads or creates a file. Arg: Filename without quotes. \n";
    }
}