namespace Points.Commands
{
    using System;
    using CommandSystem;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal sealed class Main : ParentCommand
    {
        public Main()
        {
            LoadGeneratedCommands();
        }

        public override string Command { get; } = "points";
        public override string[] Aliases { get; } = {"pnt"};
        public override string Description { get; } = "Manage Points";

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Load.Instance);
            RegisterCommand(Save.Instance);
            RegisterCommand(Mode.Instance);
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender,
            out string response)
        {
            response = "Subcommand not found. Available subcommands: load, save, mode.";
            return false;
        }
    }
}