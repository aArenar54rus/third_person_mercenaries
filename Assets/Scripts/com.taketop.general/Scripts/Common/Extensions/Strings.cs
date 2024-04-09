using UnityEngine;


namespace Module.General
{
	public static partial class Extensions
	{
		public static string Format(this string formatString, params object[] args) =>
			string.Format(formatString, args);


		public static void CopyToClipboard(this string str) =>
			GUIUtility.systemCopyBuffer = str;
	}
}
