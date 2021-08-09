using System;
using System.IO;
using Points.DataTypes;
using Points.Tools;
using Exiled.Events.EventArgs;
using UnityEngine;

namespace Points.Internal
{
	internal static class PointEditor
	{
		public static bool Enabled { get; set; }

		private const string HelpMessage =
			".pnt load 'FileName'  -FileName without quotes. Loads or creates a file.\n" +
			".pnt save             -Saves the currently loaded file.\n" +
			".pnt player           -When you add a point, use the players position as the point.\n" +
			".pnt crosshair        -When you add a point, use what your camera is pointing at as a point.\n" +
			".pnt add 'ID'         -ID without quotes. (Optional) Creates a point.";


		private static bool _useCrossHair = true;
		private static PointList _currentLoadedPointList;
		private static string _currentLoadedName;

		internal static void ServerEvents_SendingConsoleCommand(SendingConsoleCommandEventArgs ev)
		{
			if (!Enabled || !ev.Name.Equals("pnt", StringComparison.InvariantCultureIgnoreCase))
				return;

			var player = ev.Player;
			var commands = ev.Arguments;
			var commandCount = commands.Count;

			ev.IsAllowed = false;

			if (commandCount <= 0) return;
			var currentCommand = commands[0].ToLowerInvariant().Trim();

			switch (currentCommand)
			{
				case "crosshair" when _useCrossHair:
					ev.ReturnMessage = "Already using crosshair";
					break;

				case "crosshair":
					_useCrossHair = true;
					ev.ReturnMessage = "Using crosshair";
					break;

				case "help":
					ev.ReturnMessage = HelpMessage;
					break;

				case "player" when !_useCrossHair:
					ev.ReturnMessage = "Already using player position";
					break;

				case "player":
					_useCrossHair = false;
					ev.ReturnMessage = "Using player position";
					break;

				case "load":
					if (commandCount == 2)
					{
						currentCommand = commands[1];
						ev.ReturnMessage = LoadPoints(currentCommand)
							? $"{currentCommand} loaded successfully with {_currentLoadedPointList.RawPoints.Count} point(s)."
							: $"{currentCommand} does not exist, created new point list.";
					}
					else
					{
						ev.ReturnMessage = "Error: Don't use spaces in your name.";
					}

					break;

				case "save":
					ev.ReturnMessage = Save()
						? "Save Successful"
						: "Load/Create a list first!";
					break;

				case "add":

					if (commandCount > 2)
					{
						ev.ReturnMessage = "Error: Don't use spaces in your name.";
					}
					else if (_currentLoadedPointList != null)
					{

						currentCommand = commandCount == 2 ? commands[1].ToLowerInvariant() : string.Empty;

						var scp049Component = player.GameObject.GetComponent<Scp049_2PlayerScript>();
						Vector3 position;
						Vector3 rotation;

						if (_useCrossHair)
						{
							var scp106Component = player.GameObject.GetComponent<Scp106PlayerScript>();
							var cameraRotation = scp049Component.plyCam.transform.forward;
							Physics.Raycast(scp049Component.plyCam.transform.position, cameraRotation,
								out var where,
								40f, scp106Component.teleportPlacementMask);
							rotation = new Vector3(-cameraRotation.x, cameraRotation.y, -cameraRotation.z);
							position = @where.point + (Vector3.up * 0.1f);
						}
						else
						{
							var froward = scp049Component.plyCam.transform.forward;
							rotation = new Vector3(-froward.x, froward.y, -froward.z);
							position = player.Position + (Vector3.up * 0.1f);
						}

						var closestRoom = player.CurrentRoom;
						var roomName = closestRoom.Type;

						_currentLoadedPointList.RawPoints.Add(new RawPoint(currentCommand, roomName,
							closestRoom.Transform.InverseTransformPoint(position),
							closestRoom.Transform.InverseTransformDirection(rotation)));
						ev.ReturnMessage =
							$"Created point! Room: [{roomName}] ID: [{currentCommand}]";
					}
					else
					{
						ev.ReturnMessage = "Load/Create a list first!";
					}

					break;

				default:
					ev.ReturnMessage = "Invalid Command. Type .pnt help";
					break;
			}
		}

		private static bool Save()
		{
			if (_currentLoadedPointList == null) return false;
			PointIO.Save(_currentLoadedPointList, Path.Combine(PointIO.FolderPath, _currentLoadedName) + ".txt");
			return true;

		}

		/// <summary>
		/// Returns true if it found the points.
		/// </summary>
		public static bool LoadPoints(string name)
		{
			if (!Directory.Exists(PointIO.FolderPath)) Directory.CreateDirectory(PointIO.FolderPath);

			var pointList = PointIO.Open(Path.Combine(PointIO.FolderPath, name) + ".txt") ?? new PointList();

			_currentLoadedPointList = pointList;
			_currentLoadedName = name;

			if (PointManager.PointLists.ContainsKey(name))
			{
				PointManager.PointLists[name] = pointList;
				return true;
			}

			PointManager.PointLists.Add(name, pointList);
			return false;
		}
	}
}
