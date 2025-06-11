using UnityEngine;

namespace RemoteEducation.Extensions
{
    public static class Vector3Extensions
    {
		/// <returns>A copy of this Vector3 with 0 in each axis not set to true.</returns>
		public static Vector3 FlattenAxis(this Vector3 vector, bool xAxis, bool yAxis, bool zAxis)
		{
			Vector3 newVector = Vector3.zero;
			if (xAxis)
				newVector.x = vector.x;
			if (yAxis)
				newVector.y = vector.y;
			if (zAxis)
				newVector.z = vector.z;

			return newVector;
		}

		/// <summary>Calculates the reciprocal of this vector.</summary>
		/// <returns>new <see cref="Vector3"/>(1/x, 1/y, 1/z)</returns>
		public static Vector3 Reciprocal(this Vector3 vec)
		{
			return new Vector3(1 / vec.x, 1 / vec.y, 1 / vec.z);
		}

		/// <summary>
		/// Calculate the distance squared between 2 vectors.
		/// This is useful for comparing distances. It is faster than
		/// calculating the actual distance since we don't need to do a 
		/// square root calculation.
		/// </summary>
		/// <remarks>This method should be moved to the <see cref="RemoteEducation.Extensions" /> library</remarks>
		/// <param name="a">The first vector</param>
		/// <param name="b">The second vector</param>
		/// <returns>The distance between the 2 vectors squared</returns>
		public static float SqrDistance(this Vector3 start, Vector3 end)
		{
			var v = start - end;
			v.y = 0;
			return v.sqrMagnitude;
		}
	}
}
