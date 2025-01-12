using DemoDocker.Models;
using DemoDocker.Repositories.Account;

namespace DemoDocker.Repositories.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly MoviesContext _context;

		private readonly IMovieRepository _movieRepository = null!;
		private readonly IUserRepository _userRepository = null!;


		public UnitOfWork(MoviesContext context)
		{
			_context = context;
		}

		public IMovieRepository MovieRepository => _movieRepository ?? new MovieRepository(_context);
		public IUserRepository UserRepository => _userRepository ?? new UserRepository(_context);

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}

		public async Task<bool> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}
	}
}
