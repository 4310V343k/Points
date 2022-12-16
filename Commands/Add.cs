namespace Points.Commands
{
    using System;

    using CommandSystem;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using global::Points.DataTypes;
    using global::Points.Internal;

    using UnityEngine;

    internal sealed class Add : ICommand
    {
        private Add()
        {
        }

        public static Add Instance { get; } = new Add();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (PointEditor.CurrentLoadedPointList == null)
            {
                response = "Load/Create a list first!";
                return false;
            }

            var name = arguments.Count > 0 ? arguments.At(0).ToLowerInvariant() : string.Empty;
            Player player = Player.Get((CommandSender)sender);

            var scp049Component = player.GameObject.GetComponent<Scp049_2PlayerScript>();
            Vector3 position;
            Vector3 rotation;

            if (PointEditor.UseCrossHair)
            {
                Physics.Raycast(player.CameraTransform.position, player.CameraTransform.eulerAngles,
                    out var hit,
                    40f, 134217729); // hardcoded but with love
                
                // this feels wrong but eh
                Vector3 forward = player.CameraTransform.forward;
                rotation = new Vector3(-forward.x, forward.y, -forward.z);
                position = hit.point + Vector3.up * 0.1f;
            }
            else
            {
                Vector3 forward = scp049Component.plyCam.transform.forward;
                rotation = new Vector3(-forward.x, forward.y, -forward.z);
                position = player.Position + Vector3.up * 0.1f;
            }

            Room closestRoom = player.CurrentRoom;
            RoomType roomName = closestRoom.Type;

            PointEditor.CurrentLoadedPointList.RawPoints.Add(new RawPoint(name, roomName,
                closestRoom.Transform.InverseTransformPoint(position),
                closestRoom.Transform.InverseTransformDirection(rotation)));
            response =
                $"Created point! Room: [{roomName}] ID: [{name}]";
            return true;
        }

        public string Command { get; } = "add";
        public string[] Aliases { get; } = { "a" };
        public string Description { get; } = "Creates a Point. Arg: ID without quotes. (Optional)";
    }
}
