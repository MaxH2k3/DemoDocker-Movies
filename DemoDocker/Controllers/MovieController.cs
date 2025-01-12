using DemoDocker.Dto;
using DemoDocker.Helper;
using DemoDocker.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DemoDocker.Controllers
{
	[ApiController]
	[Route("api/v1/movies")]
	public class MovieController : Controller
	{
		private readonly IMovieService _movieService;
		private readonly IConfiguration _configuration;

        public MovieController(IMovieService movieService, IConfiguration configuration)
		{
			_movieService = movieService;
            _configuration = configuration;
        }

		[HttpGet("")]
		[ProducesResponseType(typeof(IEnumerable<MovieDetail>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetMovies([FromQuery] FilterMovieModel filter)
		{
			var movies = await _movieService.GetMovies(filter);

			Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(movies.Metadata));

			return Ok(SystemHelper.ConvertToSnakeCase<PagedList<MovieDetail>>(movies));
		}

		[HttpPost("")]
		[ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
		[ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> CreateMovie(NewMovie newMovie)
		{
			var response = await _movieService.CreateMovie(newMovie);

			if(response.Status == System.Net.HttpStatusCode.Created)
			{
				return Created(response.Message, response.Data);
			}

			return BadRequest(response);
		}

		[HttpPut("{movieId}")]
		[ProducesResponseType(typeof(APIResponse), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> UpdateMovie(NewMovie newMovie, Guid movieId)
		{
			newMovie.MovieId = movieId;
			var response = await _movieService.UpdateMovie(newMovie);

			if(response.Status == System.Net.HttpStatusCode.OK)
			{
				return Ok(response);
			}

			return BadRequest(response);
		}

		[HttpDelete("{movieId}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(typeof(APIResponse), StatusCodes.Status400BadRequest)]
		public async Task<IActionResult> DeleteMovie(Guid movieId)
		{
			var response = await _movieService.DeleteMovie(movieId);

			if(response.Status == System.Net.HttpStatusCode.NoContent)
			{
				return NoContent();
			}

			return BadRequest(response);
		}

		[HttpGet("{movieId}")]
		[ProducesResponseType(typeof(MovieDetail), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetMovie(Guid movieId)
		{
			var result = await _movieService.GetMovieById(movieId);

			if(result == null)
			{
				return NotFound(result);
			}

			return Ok(result);
		}

		[HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(_configuration["Test:ThisTest"]);
        }
    }
}
