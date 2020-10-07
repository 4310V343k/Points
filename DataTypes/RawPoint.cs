using System;
using Exiled.API.Enums;
using UnityEngine;

namespace ArithFeather.Points.DataTypes
{
	/// <summary>
	/// Represents a point that is a child of a <see cref="RoomType"/>.
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
		/// Gets a custom string to identify the point.
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the type of room that this point belongs to.
		/// </summary>
		public RoomType RoomType { get; }

		/// <summary>
		/// Gets the local position inside the <see cref="RoomType"/>.
		/// </summary>
		public Vector3 Position { get; }

		/// <summary>
		/// Gets the local rotation inside the <see cref="RoomType"/>.
		/// </summary>
		public Vector3 Rotation { get; }

		#region Equality

		/// <summary>
		/// This should only be called by Exiled.Points.
		/// </summary>
		internal static void ResetIds() => _uniqueIdCounter = 0;

		private static uint _uniqueIdCounter;

		private readonly uint _uniqueId;

		public bool Equals(RawPoint other) => !(other is null) && other._uniqueId == this._uniqueId;

		public override bool Equals(object obj) => obj is RawPoint point && point._uniqueId == this._uniqueId;

		public static bool operator ==(RawPoint lhs, RawPoint rhs) => !(lhs is null) && lhs.Equals(rhs);

		public static bool operator !=(RawPoint lhs, RawPoint rhs) => lhs is null || !lhs.Equals(rhs);

		public override int GetHashCode() => _uniqueId.GetHashCode();

		#endregion
	}
}