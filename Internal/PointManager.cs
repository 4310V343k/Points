namespace Points.Internal
{
    using System.Collections.Generic;
    using System.IO;

    using DataTypes;

    using Exiled.API.Features;

    using Tools;

    internal static class PointManager
    {
        public static readonly Dictionary<string, PointList> PointLists = new Dictionary<string, PointList>();

        /// <summary>
        ///     Loads all the data from the text files into <see cref="PointLists" />.
        /// </summary>
        public static void LoadData()
        {
            PointLists.Clear();

            Directory.CreateDirectory(PointIO.FolderPath);

            var files = Directory.GetFiles(PointIO.FolderPath, "*.txt");
            var fileLength = files.Length;

            for (var i = 0; i < fileLength; i++)
            {
                var filePath = files[i];
                PointList list = PointIO.Open(filePath);

                Log.Debug($"Got a raw point list: {string.Join(", ", list.RawPoints)}", Points.Singleton.Config.Debug);

                PointLists.Add(Path.GetFileNameWithoutExtension(filePath), list);
            }
        }

        /// <summary>
        ///     This will populate all the <see cref="FixedPoint" /> information inside the <see cref="PointLists" />.
        /// </summary>
        public static void SetupFixedPoints()
        {
            foreach (var list in PointLists)
            {
                Log.Debug($"Trying to fix data in {list.Key} with points: {string.Join(", ", list.Value.RawPoints)}", Points.Singleton.Config.Debug);
                list.Value.FixData();
                Log.Debug($"Fixed the data. Fixed points: {string.Join(", ", list.Value.FixedPoints)}", Points.Singleton.Config.Debug);
            }
        }
    }
}
