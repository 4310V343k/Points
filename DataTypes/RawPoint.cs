namespace Points.DataTypes
{
    using System;
    using Exiled.API.Enums;
    using UnityEngine;

    /// <summary>
    ///     Represents a point that is a child of a <see cref="RoomType" />.
    /// </summary>
    public class RawPoint : IEquatable<RawPoint>
    {
        public RawPoint(string id, RoomType roomType, Vector3 position, Vector3 rotation)
        {
            Id = id;
            RoomType = roomType;
            Position = position;
            Rotation = rotation;
            _uniqueIdCounter++;
            _uniqueId = _uniqueIdCounter;
        }

        /// <summary>
        ///     Gets a custom string to identify the point.
        /// </summary>
        public string Id { get; }

        /// <summary>
        ///     Gets the type of room that this point belongs to.
        /// </summary>
        public RoomType RoomType { get; }

        /// <summary>
        ///     Gets the local position inside the <see cref="RoomType" />.
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        ///     Gets the local rotation inside the <see cref="RoomType" />.
        /// </summary>
        public Vector3 Rotation { get; }

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
            return Equals(other as RawPoint);
        }

        public virtual bool Equals(RawPoint other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            return _uniqueId == other._uniqueId;
        }

        public static bool operator ==(RawPoint item1, RawPoint item2)
        {
            if (ReferenceEquals(item1, item2)) return true;
            if ((object) item1 == null || (object) item2 == null) return false;
            return item1._uniqueId == item2._uniqueId;
        }

        public static bool operator !=(RawPoint item1, RawPoint item2)
        {
            return !(item1 == item2);
        }

        #endregion
    }
}