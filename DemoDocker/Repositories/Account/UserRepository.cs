using DemoDocker.Models;
using DemoDocker.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace DemoDocker.Repositories.Account
{
	public class UserRepository : Repository<User>, IUserRepository
	{
		public UserRepository(MoviesContext context) : base(context)
		{
		}

		public async Task<User?> GetUserByUsernameOrEmail(string value)
		{
			return await _context.Users.FirstOrDefaultAsync(u => u.Username!.Equals(value) || u.Email!.Equals(value));
		}

		public async Task<bool> IsExisted(string email, string userName)
		{
			return await _context.Users.AnyAsync(u => u.Username!.Equals(userName) || u.Email!.Equals(email));
		}
	}
}
