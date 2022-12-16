namespace Points.DataTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using UnityEngine;

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

        public readonly List<RawPoint> RawPoints;

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
            Log.Debug($"Fixing data for id {this._uniqueId}", Points.Singleton.Config.Debug);
            FixedPoints.Clear();

            FixedPoints.Capacity = RawPoints.Count;

            Log.Debug($"Room.List.Count() = {Room.List.Count()}");
            foreach (RawPoint point in RawPoints)
            {
                var rooms = Room.Get(r => r.Type == point.RoomType);
                foreach (Room room in rooms)
                {
                    var fixedPoint = new FixedPoint(
                        point.Id,
                        room,
                        room.Transform.TransformPoint(point.Position),
                        room.Transform.TransformDirection(point.Rotation)
                    );
                    FixedPoints.Add(fixedPoint);
                }
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
