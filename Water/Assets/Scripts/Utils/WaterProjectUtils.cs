using UnityEngine;
using System.Collections;

public class WaterProjectUtils {

	/// <summary>
	/// Closests the point on line.
	/// </summary>
	/// <returns>The point on line.</returns>
	/// <param name="pointOnLine">Point on line.</param>
	/// <param name="lineDirection">Line direction.</param>
	/// <param name="point">The result will be the closest point from the line to this point</param>
	static public Vector2 ClosestPointOnLine2D(Vector2 pointOnLine, Vector2 lineDirection, Vector2 point){

		Vector2 result;

		if (lineDirection.y == 0) {
			result.y = pointOnLine.y;
			result.x = point.x;
		} else if (lineDirection.x == 0) {
			result.x = pointOnLine.x;
			result.y = point.y;
		}else
		{
			float k = lineDirection.y / lineDirection.x;
			result.x = (float)((k * pointOnLine.x + point.x / k + point.y - pointOnLine.y) / (1 / k + k));
			result.y = (float)(-1 / k * (result.x - point.x) + point.y);
		}

		return result;
	}

	/// <summary>
	/// Closests the point on line.
	/// </summary>
	/// <returns>The point on line.</returns>
	/// <param name="pointOnLine">Point1, one end of the segment</param>
	/// <param name="lineDirection">point2, the other end</param>
	/// <param name="point">The result will be the closest point from the segment to this point</param>
	static public Vector2 ClosestPointOnLineSegment2D(Vector2 point1, Vector2 point2, Vector2 point){

		Vector2 result = ClosestPointOnLine2D (point1, point2 - point1, point);

		if (Vector2.Dot (result - point1, result - point2) > 0) {
			if ((result - point1).sqrMagnitude < (result - point2).sqrMagnitude) {
				return point1;
			} else {
				return point2;
			}
		} else {
			return result;
		}

	}





	static public Vector2 GetClosestPointOnBound2D(BoxCollider2D collider, Vector2 point){
		Vector2 upRight = collider.offset + collider.size / 2;
		Vector2 downRight = upRight - Vector2.up * collider.size.y;
		Vector2 upLeft = upRight - Vector2.right * collider.size.x;
		Vector2 downLeft = collider.offset - collider.size / 2;

		Vector2[] points = new Vector2[4];
		points [0] = upRight;
		points [1] = downRight;
		points [2] = downLeft;
		points [3] = upLeft;


		return GetClosesPointFromPointsList (points, point, collider.transform);

	}

	static public Vector2 GetClosestPointOnBound2D(PolygonCollider2D collider, Vector2 point){
		return GetClosesPointFromPointsList (collider.points, point, collider.transform);

	}




	static private Vector2 GetClosesPointFromPointsList(Vector2[] points, Vector2 point, Transform transf = null){

		float minDist = Mathf.Infinity;
		Vector2 result = Vector2.zero;

		for (int i = 0; i < points.Length; i++) {

			Vector2 point1;
			Vector2 point2;
			if (transf != null) {
				point1 = transf.TransformPoint (points [i]);
			} else {
				point1 = points [i];
			}

			if (i < points.Length - 1) {
				point2 = points [i + 1];
			} else {
				point2 = points [0];
			}

			if (transf != null) {
				point2 = transf.TransformPoint (point2);
			} 


			Vector2 closestPoint = ClosestPointOnLineSegment2D(point1, point2, point);

			float dist = (closestPoint - point).sqrMagnitude;

			if(dist < minDist){
				minDist = dist;
				result = closestPoint;
			}

		}

		return result;

	}

}
