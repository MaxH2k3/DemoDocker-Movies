using DemoDocker.Models;
using DemoDocker.Repositories.Common;

namespace DemoDocker.Repositories.Account
{
	public interface IUserRepository : IRepository<User>
	{
		Task<User?> GetUserByUsernameOrEmail(string value);
		Task<bool> IsExisted(string email, string userName);

	}
}
