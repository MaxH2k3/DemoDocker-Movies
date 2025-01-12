using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DemoDocker.Enums
{
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public enum MovieSortType
	{
		[EnumMember(Value = "English Name")]
		EnglishName = 1,

		[EnumMember(Value = "Date Created")]
		DateCreated = 2,

		[EnumMember(Value = "Date Updated")]
		DateUpdated = 3,

		[EnumMember(Value = "Date Deleted")]
		DateDeleted = 4,

		[EnumMember(Value = "Produced Date")]
		ProducedDate = 5
	}
}
