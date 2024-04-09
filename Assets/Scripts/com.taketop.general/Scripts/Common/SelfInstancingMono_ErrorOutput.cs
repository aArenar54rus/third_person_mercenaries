using System.Collections.Generic;
using UnityEngine;


namespace Module.General
{
	public abstract partial class SelfInstancingMono
	{
		private static readonly List<string> preloadLogs = new List<string>();


		private static void CreateLogsOutput()
		{
			var logsObject = new GameObject("GUI Logs Output");
			DontDestroyOnLoad(logsObject);

			var logs = logsObject.AddComponent<GUILogs>();
			logs.SetMessages(preloadLogs);
		}
	}
}
