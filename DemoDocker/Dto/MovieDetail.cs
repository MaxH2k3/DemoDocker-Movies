using DemoDocker.Models;
using System.ComponentModel.DataAnnotations;

namespace DemoDocker.Dto
{
	public class MovieDetail
	{
		public Guid MovieId { get; set; }
		public double? Mark { get; set; }
		public int? Time { get; set; }
		public int? Viewer { get; set; }
		public string? Description { get; set; }
		public string? EnglishName { get; set; }
		public string? VietnamName { get; set; }
		public string? Thumbnail { get; set; }
		public string? Trailer { get; set; }
		public DateTime? ProducedDate { get; set; }
		public virtual Nation? Nation { get; set; }
		public virtual FeatureFilm? Feature { get; set; }
		public string? Status { get; set; }
		public virtual IEnumerable<Category>? Categories { get; set; }
	}
}
