using Ordering.Domain.SeedWork;
using System.Linq.Expressions;

namespace Ordering.Application.Interfaces.Repositories;

public interface IGenericRepository<T> : IRepository<T> where T : Entity
{
    Task<List<T>> GetAll();

    Task<List<T>> Get(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        params Expression<Func<T, object>>[] includes);

    Task<List<T>> Get(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);

    Task<T> GetById(Guid id);

    Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);

    Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes);

    Task<T> AddAsync(T entity);

    T Update(T entity);
}
