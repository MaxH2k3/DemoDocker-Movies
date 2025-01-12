using DemoDocker.Models;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace DemoDocker.Configuration
{
	public static class ConfigOdata
	{
		public static IEdmModel GetEdmModel()
		{
			var builder = new ODataConventionModelBuilder();

			// Register entity sets
			builder.EntitySet<Movie>("Movies");
			builder.EntitySet<User>("Users");
			builder.EntitySet<Person>("Persons");
			builder.EntitySet<Cast>("Casts");
			builder.EntitySet<Category>("Categories");
			builder.EntitySet<Episode>("Episodes");
			builder.EntitySet<FeatureFilm>("FeatureFilms");
			builder.EntitySet<MovieCategory>("MovieCategories");
			builder.EntitySet<Nation>("Nations");
			builder.EntitySet<Season>("Seasons");

			// Khai báo khóa chính cho Cast
			var castEntity = builder.EntityType<Cast>();
			castEntity.HasKey(c => new { c.ActorId, c.MovieId });

			var featureFilmEntity = builder.EntityType<FeatureFilm>();
			featureFilmEntity.HasKey(f => f.FeatureId); // Khóa chính của FeatureFilm

			var movieCategoryEntity = builder.EntityType<MovieCategory>();
			movieCategoryEntity.HasKey(mc => new { mc.MovieId, mc.CategoryId }); // Khóa chính hỗn hợp

			var nationEntity = builder.EntityType<Nation>();
			nationEntity.HasKey(f => f.NationId); // Khóa chính của FeatureFilm

			return builder.GetEdmModel();
		}
	}
}
