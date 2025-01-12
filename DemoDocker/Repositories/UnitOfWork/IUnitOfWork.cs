using DemoDocker.Repositories.Account;

namespace DemoDocker.Repositories.UnitOfWork
{
	public interface IUnitOfWork : IDisposable
	{

		IMovieRepository MovieRepository { get; }
		IUserRepository UserRepository { get; }

		Task<bool> SaveChangesAsync();
	}
}
