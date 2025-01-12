using DemoDocker.Dto;
using DemoDocker.Models;
using DemoDocker.Repositories.Common;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DemoDocker.Repositories
{
	public class MovieRepository : Repository<Movie>, IMovieRepository
	{
		public MovieRepository(MoviesContext context) : base(context)
		{
		}

		public async Task<bool> CreateMovie(Movie movie)
		{
			await _context.Movies.AddAsync(movie);

			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> DeleteMovie(Guid movieId)
		{
			var movie = await _context.Movies.FirstAsync(m => m.MovieId.Equals(movieId));

			_context.Movies.Remove(movie);

			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<bool> UpdateMovie(Movie movie)
		{
			_context.Movies.Update(movie);

			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<Movie?> GetById(Guid movieId)
		{
			return await _context.Movies
				.Include(m => m.MovieCategories)
				.Include(m => m.Nation)
				.Include(m => m.Seasons)
				.Include(m => m.Casts)
				.Include(m => m.Feature)
				.AsNoTracking()
				.AsSplitQuery()
				.FirstOrDefaultAsync(m => m.MovieId.Equals(movieId));
		}

		public IEnumerable<Movie> GetMovieRelated(Guid movieId)
		{
			var movie = _context.Movies.Include(m => m.MovieCategories)
				 .FirstOrDefault(m => m.MovieId.Equals(movieId));

			if (movie == null)
			{
				return new List<Movie>();
			}

			var moviescategories = movie.MovieCategories.Select(mc => mc.CategoryId).ToList();

			var query = $@"SELECT m.*
                FROM Movies m
                LEFT JOIN MovieCategory mc ON m.MovieID = mc.MovieID
                WHERE m.MovieID != '{movieId}'
                GROUP BY m.MovieID, m.FeatureId, m.NationID, m.Mark, m.Time, m.Viewer, m.Description, m.EnglishName, 
                m.VietnamName, m.Thumbnail, m.Trailer, m.Status, m.ProducedDate, m.DateCreated, m.DateUpdated, m.DateDeleted
                ORDER BY {(moviescategories.Count > 0 ? $"COUNT(CASE WHEN mc.CategoryID IN ({string.Join(',', moviescategories)}) THEN 1 ELSE NULL END) DESC," : "")}  m.ProducedDate DESC";

			var result = _context.Movies.FromSqlRaw(query).ToList();

			return result;
		}

		public async Task<IEnumerable<UserMonthly>> GetUserMonthly()
		{
			var query = await (
				from currentMonth in _context.Users
					.GroupBy(u => new { Year = u.DateCreated.Value.Year, Month = u.DateCreated.Value.Month })
					.Select(g => new
					{
						Year = g.Key.Year,
						Month = g.Key.Month,
						NewUsers = g.Count()
					})
				join previousMonth in _context.Users
					.GroupBy(u => new { Year = u.DateCreated.Value.Year, Month = u.DateCreated.Value.Month })
					.Select(g => new
					{
						Year = g.Key.Year,
						Month = g.Key.Month,
						NewUsers = g.Count()
					})
				on new { Year = currentMonth.Year, Month = currentMonth.Month - 1 } equals new { Year = previousMonth.Year, Month = previousMonth.Month }
				into previousMonthJoin
				from previousMonth in previousMonthJoin.DefaultIfEmpty()
				select new UserMonthly
				{
					Year = currentMonth.Year,
					Month = currentMonth.Month,
					NewUsers = currentMonth.NewUsers,
					PreviousMonthUsers = previousMonth.NewUsers,
					GrowthRate = previousMonth != null && previousMonth.NewUsers > 0
						? ((currentMonth.NewUsers - previousMonth.NewUsers) / (float)previousMonth.NewUsers) * 100
						: 0
				})
				.OrderByDescending(x => x.Year)
				.ThenByDescending(x => x.Month)
				.ToListAsync();

			return query;

		}

		public async Task<IEnumerable<AnalystMovie>> GetAnalysisMovie()
		{
			var query = await (
					from m in _context.Movies
					join s in _context.Seasons on m.MovieId equals s.MovieId into seasonGroup
					from sg in seasonGroup.DefaultIfEmpty() // Left join with Seasons
					join e in _context.Episodes on sg.SeasonId equals e.SeasonId into episodeGroup
					from eg in episodeGroup.DefaultIfEmpty() // Left join with Episodes
					group new { m, sg, eg } by new { m.MovieId, m.EnglishName } into g
					select new AnalystMovie
					{
						MovieId = g.Key.MovieId,
						MovieName = g.Key.EnglishName,
						TotalSeasons = g.Select(x => x.sg.SeasonId).Distinct().Count(),
						TotalEpisodes = g.Select(x => x.eg.EpisodeId).Distinct().Count(),
						TotalViewers = g.Sum(x => x.m.Viewer),
						AvgRating = g.Average(x => x.m.Mark)
					}
					into result
					orderby result.TotalSeasons descending, result.TotalEpisodes descending, result.AvgRating descending
					select result)
					.ToListAsync();


			return query;
		}

	}
}
