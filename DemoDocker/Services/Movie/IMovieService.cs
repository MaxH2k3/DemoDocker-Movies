using DemoDocker.Dto;
using DemoDocker.Models;

namespace DemoDocker.Services
{
	public interface IMovieService
	{
		IEnumerable<Movie> GetMovieRelated(Guid movieId);
		Task<IEnumerable<UserMonthly>> GetUserMonthly();
		Task<IEnumerable<AnalystMovie>> GetAnalystMovies();
		Task<PagedList<MovieDetail>> GetMovies(FilterMovieModel filter);
		Task<APIResponse> DeleteMovie(Guid movieId);
		Task<MovieDetail?> GetMovieById(Guid movieId);
		Task<APIResponse> UpdateMovie(NewMovie newMovie);
		Task<APIResponse> CreateMovie(NewMovie newMovie);

	}
}
