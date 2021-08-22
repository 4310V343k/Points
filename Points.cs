namespace Points
{
    using System;
    using System.Collections.Generic;
    using DataTypes;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Internal;
    using MapGeneration;
    using Server = Exiled.Events.Handlers.Server;

    /// <summary>
    ///     Use this class's static API to access point data.
    /// </summary>
    public sealed class Points : Plugin<Config>
    {
        public override Version Version => new Version(1, 2, 0);
        public override string Author => "Arith && Remindme";
        public override PluginPriority Priority => PluginPriority.First;
        public override Version RequiredExiledVersion => new Version(3, 0, 0);

        public override void OnEnabled()
        {
            Server.ReloadedConfigs += Server_ReloadedConfigs;
            SeedSynchronizer.OnMapGenerated += BeforeLoadingSpawnPoints;

            Server_ReloadedConfigs();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Server.ReloadedConfigs -= Server_ReloadedConfigs;
            SeedSynchronizer.OnMapGenerated -= BeforeLoadingSpawnPoints;

            Server_ReloadedConfigs();

            base.OnDisabled();
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
                    Log.Error(
                        "WARNING: Edit mode is enabled. Players are now able to use the console to create Points on the server.");
                else
                    Log.Warn("Edit mode is disabled.");
            }
            else
            {
                PointEditor.Enabled = false;
            }
        }

        #region API

        public delegate void LoadSpawnPoints();

        /// <summary>
        ///     This event is invoked after <see cref="PointList" /> is populated (after level generation), and before spawn points
        ///     are loaded.
        /// </summary>
        public static event LoadSpawnPoints OnLoadSpawnPoints;

        /// <summary>
        ///     This can be acquired any time after WaitingForPlayers or <see cref="OnLoadSpawnPoints" />.
        /// </summary>
        /// <param name="key">The key is the same as the file name that contains your points.</param>
        /// <returns>
        ///     <inheritdoc cref="PointList" />
        /// </returns>
        public static PointList GetPointList(string key)
        {
            if (PointManager.PointLists.TryGetValue(key, out var pointsList)) return pointsList;

            Log.Warn($"Point List: '{key}' does not exist, creating one.");
            pointsList = new PointList(new List<RawPoint>());
            PointManager.PointLists.Add(key, pointsList);
            return pointsList;
        }

        #endregion
    }
}