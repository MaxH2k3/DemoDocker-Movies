using DemoDocker.Enums;

namespace DemoDocker.Dto
{
	public class FilterMovieModel
	{
		public int Page { get; set; }
		public int EachPage { get; set; }
		public IEnumerable<int>? FeatureIds { get; set; }
		public IEnumerable<string>? NationIds { get; set; }
		public double? Mark { get; set; }
		public int? Time { get; set; }
		public int? Viewer { get; set; }
		public string? Description { get; set; }
		public string? EnglishName { get; set; }
		public string? VietnamName { get; set; }
		public MovieStatus? Status { get; set; }
		public DateTime? ProducedDate { get; set; }
		public MovieSortType? MovieSortType { get; set; }
		public bool? IsAscending { get; set; } = true;
	}
}
