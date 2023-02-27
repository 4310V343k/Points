namespace Points.Internal
{
    using System.IO;

    using DataTypes;

    using Tools;

    internal static class PointEditor
    {
        private const string HelpMessage =
            ".pnt load 'FileName'  -FileName without quotes. Loads or creates a file.\n" +
            ".pnt save             -Saves the currently loaded file.\n" +
            ".pnt player           -When you add a point, use the players position as the point.\n" +
            ".pnt crosshair        -When you add a point, use what your camera is pointing at as a point.\n" +
            ".pnt add 'ID'         -ID without quotes. (Optional) Creates a point.";


        internal static bool UseCrossHair = true;
        internal static PointList CurrentLoadedPointList;
        private static string _currentLoadedName;


        internal static bool Save()
        {
            if (CurrentLoadedPointList == null) return false;
            PointIO.Save(CurrentLoadedPointList, Path.Combine(PointIO.FolderPath, _currentLoadedName) + ".txt");
            return true;
        }

        /// <summary>
        ///     Returns true if it found the points.
        /// </summary>
        public static bool LoadPoints(string name)
        {
            if (!Directory.Exists(PointIO.FolderPath)) Directory.CreateDirectory(PointIO.FolderPath);

            PointList pointList = PointIO.Open(Path.Combine(PointIO.FolderPath, name) + ".txt") ?? new PointList();

            CurrentLoadedPointList = pointList;
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
