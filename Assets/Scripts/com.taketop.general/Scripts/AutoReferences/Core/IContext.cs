using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;


namespace Module.General.AutoReferences
{
	public interface IContext
	{
		SourceData[] GetSourcesData();
		TargetData[] GetTargetsData();
	}
}
