using DemoDocker.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace DemoDocker.Controllers
{
	[Route("odata/[controller]")]
	public class StreamMovieController : ODataController
	{
		private readonly IMovieRepository _movieRepository;

		public StreamMovieController(IMovieRepository movieRepository)
		{
			_movieRepository = movieRepository;
		}

		[EnableQuery]
		[HttpGet("")]
		public IActionResult GetMovieFilter()
		{
			return Ok(_movieRepository.GetAll());
		}
	}
}
