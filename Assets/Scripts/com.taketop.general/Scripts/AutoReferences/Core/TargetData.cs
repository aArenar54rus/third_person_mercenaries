using System;
using Object = UnityEngine.Object;


namespace Module.General.AutoReferences
{
	public class TargetData
	{
		public readonly Object TargetObject;
		public readonly TargetMembersContainer TargetMembers;


		public TargetData(Object targetObject, TargetMembersContainer targetMembers)
		{
			TargetObject = targetObject;
			TargetMembers = targetMembers;
		}
	}
}
