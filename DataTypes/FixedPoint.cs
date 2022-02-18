namespace Points.DataTypes
{
    using System;

    using Exiled.API.Features;

    using UnityEngine;

    /// <summary>
    ///     This represents a Vector3 that has been converted from local room space to world space for a specific
    ///     <see cref="Room" />.
    ///     Use these to set position/rotation of objects in game.
    /// </summary>
    public class FixedPoint : IEquatable<FixedPoint>
    {
        public FixedPoint(string id, Room room, Vector3 position, Vector3 rotation)
        {
            _uniqueIdCounter++;
            _uniqueId = _uniqueIdCounter;
            Id = id;
            Position = position;
            Rotation = Quaternion.Euler(rotation);
            Room = room;
        }

        /// <summary>
        ///     Gets the world position of the point.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        ///     Gets the world rotation of the point.
        /// </summary>
        public Quaternion Rotation { get; }

        /// <summary>
        ///     Gets the Room this point is in.
        /// </summary>
        public Room Room { get; }

        /// <summary>
        ///     Gets a custom string to identify the point.
        /// </summary>
        public string Id { get; }

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
            return Equals(other as FixedPoint);
        }

        public virtual bool Equals(FixedPoint other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _uniqueId == other._uniqueId;
        }

        public static bool operator ==(FixedPoint item1, FixedPoint item2)
        {
            if (ReferenceEquals(item1, item2)) return true;
            if ((object)item1 == null || (object)item2 == null) return false;
            return item1._uniqueId == item2._uniqueId;
        }

        public static bool operator !=(FixedPoint item1, FixedPoint item2)
        {
            return !(item1 == item2);
        }

        #endregion
    }
}
