namespace DemoDocker.Dto
{
	public class AnalystMovie
	{
		public Guid MovieId { get; set; }
		public string MovieName { get; set; } = string.Empty;
		public int TotalSeasons { get; set; }
		public int TotalEpisodes { get; set; }
		public int? TotalViewers { get; set; }
		public double? AvgRating { get; set; }
	}
}
