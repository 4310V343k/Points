using System;
using System.Collections.Generic;
using Exiled.API.Features;

namespace ArithFeather.Points.DataTypes
{
	/// <summary>
	/// This represents a collection of <see cref="RawPoint"/> which are used to make <see cref="FixedPoint"/>.
	/// You can acquire your plugin's PointList using <see cref="Points.GetPointList"/>.
	/// </summary>
	public class PointList : IEquatable<PointList>
	{
		public PointList(List<RawPoint> rawPoints = null)
		{
			_uniqueIdCounter++;
			_uniqueId = _uniqueIdCounter;
			RawPoints = rawPoints ?? new List<RawPoint>();
		}

		public readonly List<RawPoint> RawPoints;

		/// <summary>
		/// Points that have been converted from local room space to world space per room type.
		/// </summary>
		public readonly List<FixedPoint> FixedPoints = new List<FixedPoint>();

		/// <summary>
		/// Points grouped by the Room's index in <see cref="Map.Rooms"/>.
		/// Points have been converted from local room space to world space per room type.
		/// </summary>
		public readonly List<List<FixedPoint>> RoomGroupedFixedPoints = new List<List<FixedPoint>>();

		/// <summary>
		/// Points grouped by <see cref="FixedPoint.Id"/>.
		/// Points have been converted from local room space to world space per room type.
		/// </summary>
		public readonly Dictionary<string, List<FixedPoint>> IdGroupedFixedPoints = new Dictionary<string, List<FixedPoint>>();

		/// <summary>
		/// This method is used to convert all the <see cref="RawPoints"/> to <see cref="FixedPoints"/>.
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

				if (!IdGroupedFixedPoints.ContainsKey(id))
				{
					IdGroupedFixedPoints.Add(id, new List<FixedPoint>());
				}

				IdGroupedFixedPoints[id].Add(fixedPoint);
			}
		}

		#region Equality

		/// <summary>
		/// This should only be called by Exiled.Points.
		/// </summary>
		internal static void ResetIds() => _uniqueIdCounter = 0;

		private static uint _uniqueIdCounter;

		private readonly uint _uniqueId;

		public bool Equals(PointList other) => !(other is null) && other._uniqueId == this._uniqueId;

		public override bool Equals(object obj) => obj is PointList point && point._uniqueId == this._uniqueId;

		public static bool operator ==(PointList lhs, PointList rhs) => !(lhs is null) && lhs.Equals(rhs);

		public static bool operator !=(PointList lhs, PointList rhs) => lhs is null || !lhs.Equals(rhs);

		public override int GetHashCode() => _uniqueId.GetHashCode();

		#endregion
	}

}