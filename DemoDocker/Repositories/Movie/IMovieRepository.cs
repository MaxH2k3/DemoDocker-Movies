using DemoDocker.Dto;
using DemoDocker.Models;
using DemoDocker.Repositories.Common;

namespace DemoDocker.Repositories
{
	public interface IMovieRepository : IRepository<Movie>
	{
		IEnumerable<Movie> GetMovieRelated(Guid movieId);
		Task<IEnumerable<UserMonthly>> GetUserMonthly();
		Task<IEnumerable<AnalystMovie>> GetAnalysisMovie();
		Task<bool> CreateMovie(Movie movie);
		Task<bool> DeleteMovie(Guid movieId);
		Task<Movie?> GetById(Guid movieId);
		Task<bool> UpdateMovie(Movie movie);
	}
}
