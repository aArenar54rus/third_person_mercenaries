using FullSerializer;
using UnityEngine;

namespace Arenar.JSON
{
	public class FullJSONSerializer : JSONSerializer
	{
		public override string Serialize(object obj)
		{
			fsSerializer serializer = new fsSerializer();
			fsData fsData;
			serializer.TrySerialize(obj, out fsData);

			return fsData.ToString();
		}

		public override object Deserialize(string str)
		{
			return Deserialize<object>(str);
		}

		public override T Deserialize<T>(string str)
		{
			fsData parsedData = fsJsonParser.Parse(str);
			fsSerializer serializer = new fsSerializer();
			T result = default(T);
			serializer.TryDeserialize(parsedData, ref result);

			return result;
		}
	}
}
