using System;


namespace Module.General.AutoReferences
{
	public class SourceData
	{
		public readonly object SourceObject;
		public readonly Type[] SourceObjectTypes;


		public SourceData(object sourceObject, Type[] sourceObjectTypes)
		{
			SourceObject = sourceObject;
			SourceObjectTypes = sourceObjectTypes;
		}
	}
}
