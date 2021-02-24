using System;
using System.Collections.Generic;
using ArithFeather.Points.DataTypes;
using ArithFeather.Points.Internal;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using HarmonyLib;
using Server = Exiled.Events.Handlers.Server;

namespace ArithFeather.Points
{
	/// <summary>
	/// Use this class's static API to access point data.
	/// </summary>
	public sealed class Points : Plugin<Config>
	{
		private static readonly Version CurrentVersion = new Version(1, 0, 2);

		private readonly Harmony _harmony = new Harmony($"exiled.points+ {CurrentVersion}");

		public override Version Version => CurrentVersion;
		public override string Author => "Arith";
		public override PluginPriority Priority => PluginPriority.First;
		public override Version RequiredExiledVersion => new Version(2, 1, 3);

		public override void OnEnabled()
		{
			Patch();
			Server.ReloadedConfigs += Server_ReloadedConfigs;
			Server.SendingConsoleCommand += PointEditor.ServerEvents_SendingConsoleCommand;

			Server_ReloadedConfigs();

			base.OnEnabled();
		}

		public override void OnDisabled()
		{
			Unpatch();
			Server.ReloadedConfigs -= Server_ReloadedConfigs;
			Server.SendingConsoleCommand -= PointEditor.ServerEvents_SendingConsoleCommand;

			Server_ReloadedConfigs();

			base.OnDisabled();
		}

		private void Patch()
		{
			try
			{
				LoadSpawnPointPatch.OnLoadSpawnPoints += BeforeLoadingSpawnPoints;
				_harmony.PatchAll();

				Log.Debug("Events patched successfully!", Loader.ShouldDebugBeShown);
			}
			catch (Exception exception)
			{
				Log.Error($"Patching failed! {exception}");
			}
		}

		private void Unpatch()
		{
			try
			{
				Log.Debug("Unpatching events...", Loader.ShouldDebugBeShown);

				_harmony.UnpatchAll();
				LoadSpawnPointPatch.OnLoadSpawnPoints -= BeforeLoadingSpawnPoints;

				Log.Debug("All events have been unpatched complete.", Loader.ShouldDebugBeShown);
			}
			catch (Exception exception)
			{
				Log.Error($"Unpatching failed! {exception}");
			}
		}

		private void BeforeLoadingSpawnPoints()
		{
			PointManager.SetupFixedPoints();
			OnLoadSpawnPoints?.Invoke();
		}

		private void Server_ReloadedConfigs()
		{
			if (Config.IsEnabled)
			{
				PointManager.LoadData();

				PointEditor.Enabled = Config.EditMode;

				if (Config.EditMode)
				{
					Log.Error("WARNING: Edit mode is enabled. Players are now able to use the console to create Points on the server.");
				}
				else
				{
					Log.Warn("Edit mode is disabled.");
				}
			}
			else
			{
				PointEditor.Enabled = false;
			}
		}

		#region API

		public delegate void LoadSpawnPoints();

		/// <summary>
		/// This event is invoked after <see cref="PointList"/> is populated (after level generation), and before spawn points are loaded.
		/// </summary>
		public static event LoadSpawnPoints OnLoadSpawnPoints;

		/// <summary>
		/// This can be acquired any time after WaitingForPlayers or <see cref="OnLoadSpawnPoints"/>.
		/// </summary>
		/// <param name="key">The key is the same as the file name that contains your points.</param>
		/// <returns><inheritdoc cref="PointList"/></returns>
		public static PointList GetPointList(string key)
		{
			if (PointManager.PointLists.TryGetValue(key, out var pointsList))
			{
				return pointsList;
			}

			Log.Warn($"Point List: '{key}' does not exist, creating one.");
			pointsList = new PointList(new List<RawPoint>());
			PointManager.PointLists.Add(key, pointsList);
			return pointsList;
		}

		#endregion
	}
}