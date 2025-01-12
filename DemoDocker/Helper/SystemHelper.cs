using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace DemoDocker.Helper
{
	public class SystemHelper
	{
		public static string ConvertToSnakeCase<T>(T data)
		{
			var settings = new JsonSerializerSettings
			{
				ContractResolver = new DefaultContractResolver
				{
					NamingStrategy = new SnakeCaseNamingStrategy()
				},
				Formatting = Formatting.Indented
			};

			return JsonConvert.SerializeObject(data, settings);
		}
	}
}
