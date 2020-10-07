using System;
using Exiled.API.Features;
using UnityEngine;

namespace ArithFeather.Points.DataTypes
{
	/// <summary>
	/// This represents a Vector3 that has been converted from local room space to world space for a specific <see cref="Room"/>.
	/// Use these to set position/rotation of objects in game.
	/// </summary>
	public class FixedPoint : IEquatable<FixedPoint>
	{
		/// <summary>
		/// Gets the world position of the point.
		/// </summary>
		public Vector3 Position { get; }

		/// <summary>
		/// Gets the world rotation of the point.
		/// </summary>
		public Quaternion Rotation { get; }

		/// <summary>
		/// Gets the Room this point is in.
		/// </summary>
		public Room Room { get; }

		/// <summary>
		/// Gets a custom string to identify the point.
		/// </summary>
		public string Id { get; }

		public FixedPoint(string id, Room room, Vector3 position, Vector3 rotation)
		{
			_uniqueIdCounter++;
			_uniqueId = _uniqueIdCounter;
			Id = id;
			Position = position;
			Rotation = Quaternion.Euler(rotation);
			Room = room;
		}

		#region Equality

		/// <summary>
		/// This should only be called by Exiled.Points.
		/// </summary>
		internal static void ResetIds() => _uniqueIdCounter = 0;

		private static uint _uniqueIdCounter;

		private readonly uint _uniqueId;

		public bool Equals(FixedPoint other) => !(other is null) && other._uniqueId == this._uniqueId;

		public override bool Equals(object obj) => obj is FixedPoint point && point._uniqueId == this._uniqueId;

		public static bool operator ==(FixedPoint lhs, FixedPoint rhs) => !(lhs is null) && lhs.Equals(rhs);

		public static bool operator !=(FixedPoint lhs, FixedPoint rhs) => lhs is null || !lhs.Equals(rhs);

		public override int GetHashCode() => _uniqueId.GetHashCode();

#endregion
	}
}
