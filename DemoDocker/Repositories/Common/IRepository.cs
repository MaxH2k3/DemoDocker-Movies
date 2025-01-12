using DemoDocker.Dto;
using System.Linq.Expressions;

namespace DemoDocker.Repositories.Common
{
	public interface IRepository<T> where T : class
	{
		Task<T?> GetById(dynamic id);
		Task<T?> GetById(dynamic[] id);
		Task Add(T entity);
		Task Delete(dynamic id);
		Task Delete(dynamic[] id);
		Task Delete(T entity);
		Task Update(T entity);
		Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[]? includeProperties);
		Task<PagedList<T>> GetAll(int page, int eachPage, params Expression<Func<T, object>>[]? includeProperties);
		Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
											int page, int eachPage,
											params Expression<Func<T, object>>[]? includeProperties);
		Task<PagedList<T>> GetAll(int page, int eachPage,
											string sortBy, bool isAscending = false,
											params Expression<Func<T, object>>[]? includeProperties);
		Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate,
												int page, int eachPage,
												string sortBy, bool isAscending = true,
												params Expression<Func<T, object>>[]? includeProperties);
		IQueryable<T> Filter(
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			int? page = 0,
			int? eachePage = 0,
			params Expression<Func<T, object>>[]? includeProperties);
	}
}
