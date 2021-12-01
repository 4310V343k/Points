namespace Points.DataTypes
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;

    /// <summary>
    ///     This represents a collection of <see cref="RawPoint" /> which are used to make <see cref="FixedPoint" />.
    ///     You can acquire your plugin's PointList using <see cref="Points.GetPointList" />.
    /// </summary>
    public class PointList : IEquatable<PointList>
    {
        /// <summary>
        ///     Points that have been converted from local room space to world space per room type.
        /// </summary>
        public readonly List<FixedPoint> FixedPoints = new List<FixedPoint>();

        /// <summary>
        ///     Points grouped by <see cref="FixedPoint.Id" />.
        ///     Points have been converted from local room space to world space per room type.
        /// </summary>
        public readonly Dictionary<string, List<FixedPoint>> IdGroupedFixedPoints =
            new Dictionary<string, List<FixedPoint>>();

        public readonly List<RawPoint> RawPoints;

        /// <summary>
        ///     Points grouped by the Room's index in <see cref="Map.Rooms" />.
        ///     Points have been converted from local room space to world space per room type.
        /// </summary>
        public readonly List<List<FixedPoint>> RoomGroupedFixedPoints = new List<List<FixedPoint>>();

        public PointList(List<RawPoint> rawPoints = null)
        {
            _uniqueIdCounter++;
            _uniqueId = _uniqueIdCounter;
            RawPoints = rawPoints ?? new List<RawPoint>();
        }

        /// <summary>
        ///     This method is used to convert all the <see cref="RawPoints" /> to <see cref="FixedPoints" />.
        /// </summary>
        public void FixData()
        {
            FixedPoints.Clear();
            RoomGroupedFixedPoints.Clear();
            IdGroupedFixedPoints.Clear();

            var pointsCount = RawPoints.Count;
            var rooms = Map.Rooms;
            var roomCount = rooms.Count;

            FixedPoints.Capacity = pointsCount;
            RoomGroupedFixedPoints.Capacity = roomCount;

            for (var i = 0; i < roomCount; i++)
            {
                var room = rooms[i];
                var roomTransform = room.Transform;
                var roomType = room.Type;

                var pointList = new List<FixedPoint>();

                for (var j = 0; j < pointsCount; j++)
                {
                    var point = RawPoints[j];
                    var spawnRoomType = point.RoomType;

                    if (roomType == spawnRoomType)
                    {
                        var fixedPoint = new FixedPoint(
                            point.Id,
                            room,
                            roomTransform.TransformPoint(point.Position),
                            roomTransform.TransformDirection(point.Rotation)
                        );

                        pointList.Add(fixedPoint);
                        FixedPoints.Add(fixedPoint);
                    }
                }

                RoomGroupedFixedPoints.Add(pointList);
            }

            // Create Id organized rawPoints.
            var pointCount = FixedPoints.Count;
            for (var i = 0; i < pointCount; i++)
            {
                var fixedPoint = FixedPoints[i];
                var id = fixedPoint.Id;

                if (!IdGroupedFixedPoints.ContainsKey(id)) IdGroupedFixedPoints.Add(id, new List<FixedPoint>());

                IdGroupedFixedPoints[id].Add(fixedPoint);
            }
        }

        #region Equality

        /// <summary>
        ///     This should only be called by Exiled.Points.
        /// </summary>
        internal static void ResetIds()
        {
            _uniqueIdCounter = 0;
        }

        private static uint _uniqueIdCounter;

        private readonly uint _uniqueId;

        public override int GetHashCode()
        {
            return _uniqueId.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as PointList);
        }

        public virtual bool Equals(PointList other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _uniqueId == other._uniqueId;
        }

        public static bool operator ==(PointList item1, PointList item2)
        {
            if (ReferenceEquals(item1, item2)) return true;
            if ((object)item1 == null || (object)item2 == null) return false;
            return item1._uniqueId == item2._uniqueId;
        }

        public static bool operator !=(PointList item1, PointList item2)
        {
            return !(item1 == item2);
        }

        #endregion
    }
}