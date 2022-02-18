namespace Points.Internal
{
    using System.Collections.Generic;
    using System.IO;

    using global::Points.DataTypes;
    using global::Points.Tools;

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

                PointLists.Add(Path.GetFileNameWithoutExtension(filePath), list);
            }
        }

        /// <summary>
        ///     This will populate all the <see cref="FixedPoint" /> information inside the <see cref="PointLists" />.
        /// </summary>
        public static void SetupFixedPoints()
        {
            foreach (var list in PointLists) list.Value.FixData();
        }
    }
}
