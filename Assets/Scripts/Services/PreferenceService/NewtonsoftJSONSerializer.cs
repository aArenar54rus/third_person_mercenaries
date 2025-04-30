using FullSerializer;
using Newtonsoft.Json;


namespace Arenar.JSON
{
	public class NewtonsoftJSONSerializer : JSONSerializer
	{
		public override string Serialize(object obj)
		{
			var settings = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.Objects
			};

			string json = JsonConvert.SerializeObject(obj, settings);
			return json;
		}

		public override object Deserialize(string str) =>
			Deserialize<object>(str);

		public override T Deserialize<T>(string str)
		{
			var settings = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.Objects
			};

			return JsonConvert.DeserializeObject<T>(str, settings);
		}
	}
}
