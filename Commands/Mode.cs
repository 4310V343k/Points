namespace Points.Commands
{
    using System;
    using CommandSystem;
    using Internal;

    internal sealed class Mode : ICommand
    {
        private Mode()
        {
        }

        public static Mode Instance { get; } = new Mode();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = PointEditor.UseCrossHair
                ? "Using player position"
                : "Using crosshair";
            PointEditor.UseCrossHair = !PointEditor.UseCrossHair;

            return true;
        }

        public string Command { get; } = "mode";
        public string[] Aliases { get; } = { "m" };
        public string Description { get; } = "Changes how the points will be added.\n";
    }
}