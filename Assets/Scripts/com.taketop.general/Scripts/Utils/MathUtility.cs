using UnityEngine;


namespace Module.General.Utils
{
	public static class MathUtility
	{
		/// <summary>
		/// Calculates the Time elapsed factor from a given
		/// </summary>
		/// <param name="endTime">the time when the event should end</param>
		/// <param name="currentTime">current game time</param>
		/// <param name="transitionTime">The time for which the event should occur</param>
		public static float GetTimeFactor(float endTime, float currentTime, float transitionTime)
		{
			return Mathf.Clamp01(1f - (endTime - currentTime) / transitionTime);
		}


		/// <summary>
		/// Interpolates time from a curve
		/// </summary>
		/// <param name="endTime">the time when the event should end</param>
		/// <param name="currentTime">current game time</param>
		/// <param name="transitionTime">The time for which the event should occur</param>
		/// <param name="curve">The curve from which to get the value</param>
		public static float GetInterpolateTime(float endTime, float currentTime, float transitionTime, AnimationCurve curve)
		{
			float timeFactor = GetTimeFactor(endTime, currentTime, transitionTime);
			return curve.Evaluate(timeFactor);
		}
	}
}
