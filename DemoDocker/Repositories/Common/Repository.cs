using DemoDocker.Dto;
using DemoDocker.Extensions;
using DemoDocker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DemoDocker.Repositories.Common
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected readonly MoviesContext _context;

		public Repository(MoviesContext context)
		{
			_context = context;
		}

		//Base Repository
		public async Task<T?> GetById(dynamic id)
		{
			return await _context.Set<T>().FindAsync(id);
		}

		public async Task<T?> GetById(dynamic[] id)
		{
			return await _context.Set<T>().FindAsync(id);
		}

		public async Task Add(T entity)
		{
			await _context.Set<T>().AddAsync(entity);
		}

		public async Task Delete(dynamic id)
		{
			var entity = await _context.Set<T>().FindAsync(id);
			if (entity == null)
			{
				return;
			}
			_context.Set<T>().Remove(entity);
		}

		public async Task Delete(dynamic[] id)
		{
			var entity = await _context.Set<T>().FindAsync(id);
			if (entity == null)
			{
				return;
			}
			_context.Set<T>().Remove(entity);
		}

		public Task Delete(T entity)
		{
			var entry = _context.Entry(entity);
			if(entry.State == EntityState.Detached)
			{
				_context.Set<T>().Attach(entity);
			}
			_context.Set<T>().Remove(entity);

			return Task.CompletedTask;
		}

		public Task Update(T entity)
		{
			var entry = _context.Entry(entity);
			if (entry.State == EntityState.Detached)
			{
				_context.Set<T>().Attach(entity);
				entry.State = EntityState.Modified;
			}

			return Task.CompletedTask;
		}

		//Custom Repository Get All

		public async Task<IEnumerable<T>> GetAll(params Expression<Func<T, object>>[]? includeProperties)
		{
			var query = _context.Set<T>().AsQueryable();

			if (includeProperties != null)
			{
				foreach (var includeProperty in includeProperties)
				{
					query = query.Include(includeProperty);
				}
			}

			return await query.ToListAsync();
		}

		public async Task<PagedList<T>> GetAll(int page, int eachPage, params Expression<Func<T, object>>[]? includeProperties)
		{
			var query = _context.Set<T>().AsQueryable();

			if(includeProperties != null)
			{
				foreach (var includeProperty in includeProperties)
				{
					query = query.Include(includeProperty);
				}
			}

			return await query.ToPagedListAsync(page, eachPage);
		}

		public async Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate, 
											int page, int eachPage, 
											params Expression<Func<T, object>>[]? includeProperties)
		{
			var query = _context.Set<T>()
				.Where(predicate)
				.AsQueryable();

			if (includeProperties != null)
			{
				foreach (var includeProperty in includeProperties)
				{
					query = query.Include(includeProperty);
				}
			}

			return await query.ToPagedListAsync(page, eachPage);
		}

		public async Task<PagedList<T>> GetAll(int page, int eachPage, 
											string sortBy, bool isAscending = false,
											params Expression<Func<T, object>>[]? includeProperties)
		{
			var query = _context.Set<T>().AsQueryable();

			if (includeProperties != null)
			{
				foreach (var includeProperty in includeProperties)
				{
					query = query.Include(includeProperty);
				}
			}

			var result = query.ToPaginateAndSort(page, eachPage, sortBy, isAscending);

			return await Task.FromResult(result);

		}

		public async Task<PagedList<T>> GetAll(Expression<Func<T, bool>> predicate, 
												int page, int eachPage, 
												string sortBy, bool isAscending = true,
												params Expression<Func<T, object>>[]? includeProperties)
		{
			var query = _context.Set<T>()
				.Where(predicate)
				.AsQueryable()
				.AsNoTracking()
				.AsSplitQuery();

			if (includeProperties != null)
			{
				foreach (var includeProperty in includeProperties)
				{
					query = query.Include(includeProperty);
				}
			}

			var result = query.ToPaginateAndSort(page, eachPage, sortBy, isAscending);

			return await Task.FromResult(result);

		}

		public IQueryable<T> Filter(
			Expression<Func<T, bool>>? filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			int? page = 0,
			int? eachePage = 0,
			params Expression<Func<T, object>>[]? includeProperties)
		{
			IQueryable<T> query = _context.Set<T>().AsQueryable().AsNoTracking();


			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (includeProperties != null)
			{
				foreach (var includeProperty in includeProperties)
				{
					query = query.Include(includeProperty);
				}
			}

			if (orderBy != null)
			{
				orderBy(query);
			}

			if (page > 0)
			{
				query.Skip(page.Value);
			}
			if (eachePage > 0)
			{
				query.Take(eachePage.Value);
			}

			return query;
		}
	}
}
