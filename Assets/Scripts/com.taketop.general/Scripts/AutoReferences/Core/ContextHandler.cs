using System;
using System.Collections.Generic;


namespace Module.General.AutoReferences
{
	internal static class ContextHandler
	{
		public static event Action<IContext> OnAddContext;
		public static event Action<IContext> OnRemoveContext;

		private static List<IContext> contextList = new List<IContext>();


		public static void Add(IContext context)
		{
			if (contextList.Contains(context))
			{
				return;
			}

			contextList.Add(context);
			OnAddContext?.Invoke(context);
		}


		public static void Remove(IContext context)
		{
			if (!contextList.Contains(context))
			{
				return;
			}

			contextList.Remove(context);
			OnRemoveContext?.Invoke(context);
		}


		public static IEnumerable<IContext> GetCurrentContexts()
		{
			return contextList.ToArray();
		}
	}
}
