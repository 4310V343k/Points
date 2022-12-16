namespace Points
{
    using System;
    using System.Collections.Generic;

    using DataTypes;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events;
    using Exiled.Events.Extensions;

    using Internal;

    using MEC;

    using Server = Exiled.Events.Handlers.Server;

    /// <summary>
    ///     Use this class's static API to access point data.
    /// </summary>
    public sealed class Points : Plugin<Config>
    {
        public static Points Singleton;
        public override Version Version => new Version(1, 3, 0);
        public override string Author => "Arith && Remindme";
        public override PluginPriority Priority => PluginPriority.First;
        public override Version RequiredExiledVersion => new Version(5, 0, 0);

        public override void OnEnabled()
        {
            Singleton = this;

            Server.ReloadedConfigs += OnReloadedConfigs;
            Server.WaitingForPlayers += LoadSpawnPoints;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Server.ReloadedConfigs -= OnReloadedConfigs;
            Server.WaitingForPlayers -= LoadSpawnPoints;

            OnReloadedConfigs();

            Singleton = null;
            base.OnDisabled();
        }

        public void LoadSpawnPoints()
        {
            Timing.CallDelayed(1f, () =>
            {
                OnReloadedConfigs();
                Log.Debug("LoadSpawnPoints", Singleton.Config.Debug);
                PointManager.SetupFixedPoints();
                LoadedSpawnPoints?.InvokeSafely();
            });
        }

        private void OnReloadedConfigs()
        {
            if (Config.IsEnabled)
            {
                PointManager.LoadData();

                PointEditor.Enabled = Config.EditMode;

                if (Config.EditMode)
                    Log.Error(
                        "WARNING: Edit mode is enabled. Players are now able to use the console to create Points on the server.");
                else
                    Log.Debug("Edit mode is disabled.", Config.Debug);
            }
            else
            {
                PointEditor.Enabled = false;
            }
        }

        #region API

        /// <summary>
        ///     This event is invoked after <see cref="PointList" /> is populated (after level generation), and before spawn points
        ///     are loaded.
        /// </summary>
        public static event Events.CustomEventHandler LoadedSpawnPoints;

        /// <summary>
        ///     This can be acquired any time after WaitingForPlayers or <see cref="LoadedSpawnPoints" />.
        /// </summary>
        /// <param name="key">The key is the same as the file name that contains your points.</param>
        /// <returns>
        ///     <inheritdoc cref="PointList" />
        /// </returns>
        public static PointList GetPointList(string key)
        {
            if (PointManager.PointLists.TryGetValue(key, out PointList pointsList)) return pointsList;

            Log.Warn($"Point List: '{key}' does not exist, creating one.");
            pointsList = new PointList(new List<RawPoint>());
            PointManager.PointLists.Add(key, pointsList);
            return pointsList;
        }

        #endregion
    }
}
