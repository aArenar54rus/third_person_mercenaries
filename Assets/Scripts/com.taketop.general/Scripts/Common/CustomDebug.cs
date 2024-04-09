using UnityEngine;


namespace Module.General
{
	public sealed class CustomDebug
	{
		internal static bool Enable
		{
			get
			{
				bool result = false;

				#if UNITY_EDITOR || !BUILD_TYPE_PUBLISHER
				result = true;
				#endif

				return result;
			}
		}


		public static void Log(object message)
		{
			if (Enable)
			{
				Debug.Log(message);
			}
		}


		public static void Log(object message, Object context)
		{
			if (Enable)
			{
				Debug.Log(message, context);
			}
		}


		public static void LogWarning(object message)
		{
			if (Enable)
			{
				Debug.LogWarning(message);
			}
		}


		public static void LogWarning(object message, Object context)
		{
			if (Enable)
			{
				Debug.LogWarning(message, context);
			}
		}


		public static void LogError(object message)
		{
			if (Enable)
			{
				Debug.LogError(message);
			}
		}


		public static void LogError(object message, Object context)
		{
			if (Enable)
			{
				Debug.LogError(message, context);
			}
		}


		public static void LogFormat(string format, params object[] args)
		{
			if (Enable)
			{
				Debug.LogFormat(format, args);
			}
		}


		public static void LogWarningFormat(string format, params object[] args)
		{
			if (Enable)
			{
				Debug.LogWarningFormat(format, args);
			}
		}


		public static void LogErrorFormat(string format, params object[] args)
		{
			if (Enable)
			{
				Debug.LogErrorFormat(format, args);
			}
		}
	}
}
