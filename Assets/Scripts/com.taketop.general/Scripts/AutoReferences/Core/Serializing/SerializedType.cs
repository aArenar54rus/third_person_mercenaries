using System;
using System.Collections.Generic;
using UnityEngine;


namespace Module.General.AutoReferences
{
	[Serializable]
	public class SerializedType : ISerializationCallbackReceiver
	{
		private static readonly Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

		private const string EMPTY_CLASS = "(None)";
		private const string DESERIALIZE_EXCEPTION_MESSAGE = "Serialized class cannot be deserialized";
		private const string SEPARATOR = ", ";

		[SerializeField] private string serializedName = default;

		private Type type;


		public Type Type
		{
			get => type;
			private set
			{
				type = value ?? throw new NullReferenceException();
				serializedName = GetSerializedName(value);
				typeMap[serializedName] = type;
			}
		}


		public SerializedType() {}


		public SerializedType(string assemblyQualifiedClassName)
		{
			Type = !string.IsNullOrEmpty(assemblyQualifiedClassName)
				? Type.GetType(assemblyQualifiedClassName)
				: null;
		}


		public SerializedType(Type type)
		{
			Type = type;
		}


		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (!string.IsNullOrEmpty(serializedName))
			{
				if (typeMap.TryGetValue(serializedName, out type))
				{
					return;
				}

				type = Type.GetType(serializedName);
				if (type == null)
				{
					throw new ArgumentException(DESERIALIZE_EXCEPTION_MESSAGE, serializedName);
				}

				typeMap.Add(serializedName, type);
			}
			else
			{
				type = null;
			}
		}


		void ISerializationCallbackReceiver.OnBeforeSerialize() {}


		public static implicit operator string(SerializedType typeReference)
		{
			return typeReference.serializedName;
		}


		public static implicit operator Type(SerializedType typeReference)
		{
			return typeReference.Type;
		}


		public static implicit operator SerializedType(Type type)
		{
			return new SerializedType(type);
		}


		public override string ToString()
		{
			if (type == null)
			{
				return EMPTY_CLASS;
			}

			string fullName = type.FullName;

			return string.IsNullOrEmpty(fullName) ? EMPTY_CLASS : fullName;
		}


		private static string GetSerializedName(Type type)
		{
			return type != null
				? type.FullName + SEPARATOR + type.Assembly.GetName().Name
				: string.Empty;
		}
	}
}
