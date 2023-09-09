namespace Points
{
    using System;
    using System.Collections.Generic;

    using DataTypes;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.Features;

    using Internal;

    using Server = Exiled.Events.Handlers.Server;

    /// <summary>
    ///     Use this class's static API to access point data.
    /// </summary>
    public sealed class Points : Plugin<Config>
    {
        public override Version Version => new Version(1, 6, 0);
        public override string Author => "Arith && Remindme";
        public override PluginPriority Priority => PluginPriority.First;
        public override Version RequiredExiledVersion => new Version(8, 0, 0);

        public override void OnEnabled()
        {
            Server.ReloadedConfigs += ReloadPoints;
            Server.WaitingForPlayers += ReloadPoints;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Server.ReloadedConfigs -= ReloadPoints;
            Server.WaitingForPlayers -= ReloadPoints;

            base.OnDisabled();
        }

        private static void ReloadPoints()
        {
            PointManager.LoadData();
            PointManager.SetupFixedPoints();
            LoadedSpawnPoints?.InvokeSafely();
        }

        #region API

        /// <summary>
        ///     This event is invoked after <see cref="PointList" /> is populated (after level generation)
        /// </summary>
        public static Event LoadedSpawnPoints { get; set; } = new Event();

        /// <summary>
        ///     This can be acquired any time after <see cref="LoadedSpawnPoints" />.
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
