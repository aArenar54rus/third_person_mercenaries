using UnityEngine;


namespace Module.General
{
	public static partial class Extensions
	{
		public static Vector2 To2D(this Vector3 vector) =>
			new Vector2(vector.x, vector.z);


		public static Vector3 To3D(this Vector2 vector) =>
			new Vector3(vector.x, 0, vector.y);


		public static Vector3 ToZeroY(this Vector3 vector) =>
			new Vector3(vector.x, 0, vector.z);


		public static float GetMin(this Vector2 vector) =>
			Mathf.Min(vector.x, vector.y);


		public static float GetMin(this Vector3 vector) =>
			Mathf.Min(vector.x, vector.y, vector.z);


		public static int GetMin(this Vector2Int vector) =>
			Mathf.Min(vector.x, vector.y);


		public static int GetMin(this Vector3Int vector) =>
			Mathf.Min(vector.x, vector.y, vector.z);


		public static float GetMiddleValue(this Vector3 vector) =>
			(vector.x + vector.y + vector.z) / 3;


		public static float GetMax(this Vector2 vector) =>
			Mathf.Max(vector.x, vector.y);


		public static float GetMax(this Vector3 vector) =>
			Mathf.Max(vector.x, vector.y, vector.z);


		public static int GetMax(this Vector2Int vector) =>
			Mathf.Max(vector.x, vector.y);


		public static int GetMax(this Vector3Int vector) =>
			Mathf.Max(vector.x, vector.y, vector.z);


		public static float GetRandom(this Vector2 vector) =>
			Random.Range(vector.x, vector.y);


		public static int GetRandom(this Vector2Int vector) =>
			Random.Range(vector.x, vector.y);


		public static Vector2 Sort(this Vector2 vector) =>
			vector.y < vector.x
				? new Vector2(vector.y, vector.x)
				: vector;


		public static Vector2 Abs(this Vector2 vector) =>
			new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));


		public static Vector3 Abs(this Vector3 vector) =>
			new Vector3(Mathf.Abs(vector.x), Mathf.Abs(vector.y), Mathf.Abs(vector.z));


		public static float Distance2D(this Vector3 a, Vector3 b) =>
			Vector2.Distance(a.To2D(), b.To2D());


		public static float Distance2D(this Vector3 a, Vector2 b) =>
			Vector2.Distance(a.To2D(), b);


		public static float Distance2D(this Vector2 a, Vector3 b) =>
			Vector2.Distance(a, b.To2D());


		public static float SqrMagnitude2D(this Vector3 a, Vector3 b) =>
			(a.To2D() - b.To2D()).sqrMagnitude;


		public static float SqrMagnitude2D(this Vector3 a, Vector2 b) =>
			(a.To2D() - b).sqrMagnitude;


		public static float SqrMagnitude2D(this Vector2 a, Vector3 b) =>
			(a - b.To2D()).sqrMagnitude;
	}
}
