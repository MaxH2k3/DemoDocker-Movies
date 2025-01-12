using AutoMapper;
using DemoDocker.Dto;
using DemoDocker.Enums;
using DemoDocker.Models;
using DemoDocker.Repositories;
using DemoDocker.Repositories.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Net;

namespace DemoDocker.Services
{
	public class MovieService : IMovieService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public MovieService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<AnalystMovie>> GetAnalystMovies()
		{
			return await _unitOfWork.MovieRepository.GetAnalysisMovie();
		}

		public async Task<IEnumerable<UserMonthly>> GetUserMonthly()
		{
			return await _unitOfWork.MovieRepository.GetUserMonthly();
		}

		public IEnumerable<Movie> GetMovieRelated(Guid movieId)
		{
			return _unitOfWork.MovieRepository.GetMovieRelated(movieId);
		}

		public async Task<PagedList<MovieDetail>> GetMovies(FilterMovieModel filter)
		{
			var result = await _unitOfWork.MovieRepository.GetAll(
				m =>
				(filter.EnglishName == null || m.EnglishName!.Contains(filter.EnglishName)) &&
				(filter.VietnamName == null || m.VietnamName!.Contains(filter.VietnamName)) &&
				(filter.Description == null || m.Description!.Contains(filter.Description)) &&
				(filter.Status == null || m.Status!.Equals(filter.Status.ToString())) &&
				(filter.Mark == null || filter.Mark == m.Mark) &&
				(filter.Viewer == null || filter.Viewer == m.Viewer) &&
				(filter.FeatureIds.IsNullOrEmpty() || filter.FeatureIds!.Contains(m.FeatureId)) && 
				(filter.NationIds.IsNullOrEmpty() || filter.NationIds!.Contains(m.NationId)) &&
				(filter.ProducedDate == null || filter.ProducedDate <= DateTime.Now),
				filter.Page,
				filter.EachPage,
				filter.MovieSortType?.ToString() ?? MovieSortType.ProducedDate.ToString(),
				filter?.IsAscending ?? true,
				p => p.MovieCategories,
				p => p.Feature!,
				p => p.Nation!,
				p => p.Casts
				);

			return _mapper.Map<PagedList<MovieDetail>>(result);
		}

		public async Task<APIResponse> DeleteMovie(Guid movieId)
		{
			await _unitOfWork.MovieRepository.Delete(movieId);

			if(await _unitOfWork.SaveChangesAsync())
			{
				return new APIResponse()
				{
					Status = System.Net.HttpStatusCode.NoContent,
					Message = "Delete Successfully"
				};
			}

			return new APIResponse()
			{
				Status = System.Net.HttpStatusCode.BadRequest,
				Message = "Error while saving..."
			};
		}

		public async Task<MovieDetail?> GetMovieById(Guid movieId)
		{
			var movie = await _unitOfWork.MovieRepository.GetById(movieId);

			if(movie == null)
			{
				return null;
			}

			return _mapper.Map<MovieDetail>(movie);
		}

		public async Task<APIResponse> UpdateMovie(NewMovie newMovie)
		{
			Movie? movie = await _unitOfWork.MovieRepository.GetById((Guid)newMovie.MovieId!);
			DateTime? DateCreated = movie?.DateCreated;
			string? Status = movie?.Status;
			if (movie == null)
			{
				return new APIResponse(HttpStatusCode.NotFound, "Movie not found");
			}

			movie = _mapper.Map<Movie>(newMovie);
			movie.NationId = movie.NationId?.ToUpper();
			movie.DateCreated = DateCreated;
			movie.Status = Status;
			movie.DateUpdated = DateTime.Now;

			await _unitOfWork.MovieRepository.Update(movie);

			if (await _unitOfWork.SaveChangesAsync())
			{
				return new APIResponse(HttpStatusCode.OK, "Update movie successfully!");
			}

			return new APIResponse(HttpStatusCode.ServiceUnavailable, "Server error!");
		}

		public async Task<APIResponse> CreateMovie(NewMovie newMovie)
		{

			newMovie.MovieId = Guid.NewGuid();

			Movie movie = new Movie();
			movie = _mapper.Map<Movie>(newMovie);
			movie.Status = MovieStatus.Pending.ToString();
			movie.NationId = movie.NationId?.ToUpper();
			movie.DateCreated = DateTime.Now;
			movie.DateUpdated = DateTime.Now;

			APIResponse response;

			 await _unitOfWork.MovieRepository.Add(movie);

			if (await _unitOfWork.SaveChangesAsync())
			{
				response = new APIResponse(HttpStatusCode.Created, "Create movie successfully!", newMovie.MovieId);
			} else
			{
				response = new APIResponse(HttpStatusCode.ServiceUnavailable, "Server error!");
			}

			return response;
		}
	}
}
