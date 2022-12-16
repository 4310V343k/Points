namespace Points.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using global::Points.DataTypes;

    using UnityEngine;

    /// <summary>
    ///     For Saving and Loading FixedPoint Data.
    /// </summary>
    public static class PointIO
    {
        /// <summary>
        ///     The default path to the "RawPoint" folder where all point data is saved.
        /// </summary>
        public static readonly string FolderPath = Path.Combine(Paths.Configs, "PointsData");

        /// <summary>
        ///     Get a <see cref="PointList" /> from a file path only populated with <see cref="RawPoint" />.
        /// </summary>
        /// <param name="filePath">
        ///     The complete path to the file. You can use <see cref="FolderPath" /> to get the default RawPoint
        ///     folder.
        /// </param>
        public static PointList Open(string filePath)
        {
            var data = new List<RawPoint>();
            try
            {
                if (FileManager.FileExists(filePath))
                    using (StreamReader reader = File.OpenText(filePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            var item = reader.ReadLine()?.ToLowerInvariant();

                            if (string.IsNullOrWhiteSpace(item) || item.StartsWith("#"))
                                continue;

                            var sData = item.Split(':');
                            var sDataLength = sData.Length;

                            if (sDataLength != 4)
                            {
                                ThrowLoadError(item,
                                    sDataLength > 4 ? "Too many ':' splitters." : "Not enough ':' split data.");
                                continue;
                            }

                            var id = sData[0].Trim();

                            RoomType room = Enum.TryParse(sData[1].Trim(), true, out RoomType roomType)
                                ? roomType
                                : RoomType.Unnamed;

                            if (room == RoomType.Unnamed) Log.Warn($"Room unknown: [{item}]");

                            if (TryParseVector3(sData[2].Trim(), out Vector3 position) &&
                                TryParseVector3(sData[3].Trim(), out Vector3 rotation))
                                data.Add(new RawPoint(id, room, position, rotation));
                            else
                                ThrowLoadError(item, "Vector3 data is invalid.");
                        }

                        if (data.Count > 0) return new PointList(data);
                    }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            return null;
        }

        private static void ThrowLoadError(string item, string message = "")
        {
            Log.Error($"Could not load line: [{item}] ({message})");
        }

        /// <summary>
        ///     Saves a <see cref="PointList" />'s <see cref="RawPoint" /> to a file.
        /// </summary>
        /// <param name="pointList">The PointList so save.</param>
        /// <param name="filePath">
        ///     The complete path to the file. You can use <see cref="FolderPath" /> to get the default RawPoint
        ///     folder.
        /// </param>
        public static void Save(PointList pointList, string filePath)
        {
            try
            {
                var culture = CultureInfo.GetCultureInfo("en-US");
                var data = pointList.RawPoints;
                using (var writer = new StreamWriter(File.Create(filePath)))
                {
                    foreach (RawPoint point in data)
                    {
                        Vector3 pos = point.Position.RoundVector3();
                        Vector3 rot = point.Rotation.RoundVector3();
                        writer.WriteLine(
                            $"{point.Id}:{point.RoomType}:{pos.x.ToString(culture)},{pos.y.ToString(culture)},{pos.z.ToString(culture)}:{rot.x.ToString(culture)},{rot.y.ToString(culture)},{rot.z.ToString(culture)}");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"Could not save Raw Point data: {e.Message}");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryParseVector3(string vectorData, out Vector3 vector)
        {
            vector = Vector3.zero;
            var sVector = vectorData.Split(',');

            if (sVector.Length != 3)
                return false;

            if (!float.TryParse(sVector[0].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var x)
                || !float.TryParse(sVector[1].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var y)
                || !float.TryParse(sVector[2].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var z))
                return false;

            vector = new Vector3(x, y, z);
            return true;
        }
    }
}
